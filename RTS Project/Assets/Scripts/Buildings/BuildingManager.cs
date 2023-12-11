using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BuildingManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Building[] buildings;
    [SerializeField] private ResourceItemManager resources;
    [SerializeField] private Terrain terrain;
    [SerializeField] private PointManager pointManager;

    [Header("Build Progresses")]
    [SerializeField] private GameObject buildProgressPrefab;
    [SerializeField] private float buildProgressHeight;

    [Header("Particle")]
    [SerializeField] public GameObject buildParticle;

    [Header("Materials")]
    [SerializeField] public Material buildingMaterial;
    [SerializeField] private Material correctPlaceMaterial;
    [SerializeField] private Material incorrectPlaceMaterial;

    [Header("Build Errors")]
    [SerializeField] private GameObject buildErrorPrefab;
    [SerializeField] private Transform buildErrorParent;

    [Header("Variables")]
    [SerializeField] private LayerMask buildLayerMask;
    [SerializeField] private LayerMask tempBuildingLayerMask;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float degreesToRotate = 90f;
    [SerializeField] private float buildErrorFloatUpSpeed;
    [SerializeField] private float maxAngle;
    [SerializeField] private float maxHeight;
    [SerializeField] private float minHeight;

    public GameObject pendingObject;
    private int currentIndex = -1;
    //private Vector3 pos;
    private RaycastHit hit;
    private bool rayHit;

    public NetworkVariable<Vector3> pos = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> rot = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> canPlace = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [System.Serializable]
    class Building
    {
        public GameObject building;
        public float yHeight;
        public bool multiPlace;
        public Gradient buildParticleRandomColor;
        public float buildTime;
        public bool isUnlocked;
        public int[] buildingsToUnlock;
        public Button button;
    }

    private void Start()
    {
        UpdateButtons();
    }


    private void Awake()
    {
        terrain = FindObjectOfType<Terrain>().GetComponent<Terrain>();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            GetComponent<BuildingManager>().enabled = false;
        }
        UpdateButtons();
    }

    public void UpdateButtons()
    {
        //if (!IsOwner) return;
        for (int i = 0; i < buildings.Length; i++)
        {
            Button button = buildings[i].button;
            button.interactable = false;
            button.onClick.RemoveAllListeners();
            int index = i;
            button.onClick.AddListener(() => SelectObject(index));

            if (buildings[i].isUnlocked)
            {
                button.interactable = true;
            }
        }
    }

    void Update()
    {
        if (!IsOwner) return;
        if (currentIndex < 0) return;
        if (!pendingObject) return;

        pendingObject.transform.position = pos.Value;
        rot.Value = pendingObject.transform.rotation;

        CheckCanPlace(false);
        //Change pending object material based on if it can be placed or not
        if (!canPlace.Value)
        {
            ChangeObjectMaterial(pendingObject, incorrectPlaceMaterial);
        }
        else
        {
            ChangeObjectMaterial(pendingObject, correctPlaceMaterial);
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            pendingObject.SetActive(true);
            rayHit = true;

            //check raycast for terrain hit normal and check if can place

            Vector3 gridPos = Vector3Int.RoundToInt(hit.point);
            if (terrain)
            {
                gridPos.y = terrain.SampleHeight(gridPos) + (buildings[currentIndex].building.transform.localScale.y / 2);
            }
            else
            {
                gridPos.y = buildings[currentIndex].building.transform.localScale.y;
            }
            pos.Value = gridPos;

            //rotate object towards hit.normal
        }
        else
        {
            pendingObject.SetActive(false);
            rayHit = false;
        }

        HandleInput();
    }

    private void HandleInput()
    {
        //Destroy and reset pending object
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetObjectServerRpc();
        }
        //Rotate pending object
        else if (Input.GetKeyDown(KeyCode.R))
        {
            //Reverse rotation when holding leftshift
            if (Input.GetKey(KeyCode.LeftShift))
            {
                pendingObject.transform.Rotate(Vector3.up * -degreesToRotate);

            }
            else
            {
                pendingObject.transform.Rotate(Vector3.up * degreesToRotate);

            }
        }

        //place object
        if (Input.GetMouseButtonDown(0))
        {
            CheckCanPlace(true);
            if (canPlace.Value)
            {
                BuildObjectServerRpc();
            }
        }
    }
    private void CheckCanPlace(bool spawnError)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            canPlace.Value = false;
            return;
        }

        if (!rayHit)
        {
            canPlace.Value = false;
            return;
        }
        float rayAngle = Vector3.Angle(pendingObject.transform.up, Vector3.up);

        if (rayAngle > maxAngle)
        {
            if (spawnError)
            {
                SpawnError("angle too steep");
                canPlace.Value = false;

            }
            canPlace.Value = false;
            return;
        }

        if (pendingObject.transform.position.y > maxHeight)
        {
            if (spawnError)
            {
                //TODO: improve error
                SpawnError("Too high");
            }
            canPlace.Value = false;
            return;
        }

        if (pendingObject.transform.position.y < minHeight)
        {
            if (spawnError)
            {
                //TODO: improve error
                SpawnError("Too low");
            }
            canPlace.Value = false;
            return;
        }

        pendingObject.SetActive(true);
        //check collision
        if (GetOccupany(pendingObject) || !rayHit)
        {
            if (spawnError)
            {
                SpawnError("Can't place building here");
            }
            canPlace.Value = false;
            return;
        }

        bool hasEverything = true;

        List<ItemSlot> savedSlots = new();

        //loop through all recipe items and all resources and check if the player has enough resources to build the building
        foreach (var itemNeeded in buildings[currentIndex].building.GetComponent<BuildingBase>().GetRecipes())
        {
            resources.data.Value = itemNeeded.data;
            resources.GetSlotByItemDataServerRpc();

            if (itemNeeded.data == resources.itemSlotVar.Value.data)
            {
                
                if (resources.itemSlotVar.Value.amount >= itemNeeded.amountNeeded)
                {
                    savedSlots.Add(resources.itemSlotVar.Value);
                    hasEverything = true;
                }
                else
                {
                    if (spawnError)
                    {
                        SpawnError($"needs {itemNeeded.amountNeeded - resources.itemSlotVar.Value.amount} " +
                            $"more : {itemNeeded.data.name}");
                    }
                    hasEverything = false;
                    canPlace.Value = false;
                    return;
                }
            }
        }

        //remove items only when the player can actually build it
        if (hasEverything)
        {
            savedSlots.Clear();

            //build object
            canPlace.Value = true;
            return;
        }

        if (spawnError)
        {
            SpawnError("Other error");
        }
        canPlace.Value = false;
        return;
    }

    private void SpawnError(string text)
    {
        GameObject buildError = Instantiate(buildErrorPrefab, buildErrorParent);
        StartCoroutine(ErrorHandle(buildError, text));
    }

    IEnumerator ErrorHandle(GameObject buildError, string text)
    {
        TMP_Text buildErrorText = buildError.GetComponent<TMP_Text>();
        buildErrorText.SetText(text);

        yield return new WaitForSeconds(1);

        while (buildError)
        {
            buildErrorText.DOFade(0f, 1f).OnComplete(() => Destroy(buildError));
            buildError.transform.Translate(Vector2.up * buildErrorFloatUpSpeed);

            yield return null;
        }

        yield return null;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResetObjectServerRpc()
    {
        Destroy(pendingObject);
        pendingObject = null;
        currentIndex = -1;
        //pos.Value = Vector3.zero;
    }

    [ServerRpc(RequireOwnership = false)]
    private void BuildObjectServerRpc()
    {
        currentIndex = 0;
        foreach (var itemNeeded in buildings[currentIndex].building.GetComponent<BuildingBase>().GetRecipes())
        {
            resources.data.Value = itemNeeded.data;
            resources.GetSlotByItemDataServerRpc();

            if (itemNeeded.data == resources.itemSlotVar.Value.data)
            {
                resources.itemSlotVar.Value.amount -= itemNeeded.amountNeeded;
            }
        }
        //endingObjectVector3.Value = pendingObject.transform.position;

        ParticleSystem spawnedParticle = Instantiate(buildParticle, pos.Value, Quaternion.identity).GetComponent<ParticleSystem>();
        spawnedParticle.Play();

        print("Pending object: " + pendingObject);
        print("Building to spawn: " + buildings[currentIndex].building);
        BuildingBase spawnedBuilding = Instantiate(buildings[currentIndex].building, pos.Value, rot.Value).GetComponent<BuildingBase>();
        //BuildingBase spawnedBuilding = Instantiate(buildings[currentIndex].building).GetComponent<BuildingBase>();
        spawnedBuilding.GetComponent<NetworkObject>().Spawn(true);


        spawnedBuilding.InitClientRpc(buildings[currentIndex].buildTime, BuildingBase.States.Building);

        spawnedBuilding.SetOccupancyType(BuildingBase.OccupancyType.Player);
        spawnedBuilding.BuildClientRpc();

        BuildProgress buildProgress = Instantiate(buildProgressPrefab, new Vector3(spawnedBuilding.transform.position.x,
            spawnedBuilding.transform.position.y + spawnedBuilding.transform.localScale.y + buildProgressHeight,
            spawnedBuilding.transform.position.z), Quaternion.identity, spawnedBuilding.transform).GetComponent<BuildProgress>();
        buildProgress.Init(buildings[currentIndex].buildTime);

        for (int i = 0; i < buildings[currentIndex].buildingsToUnlock.Length; i++)
        {
            buildings[buildings[currentIndex].buildingsToUnlock[i]].isUnlocked = true;
        }

        pointManager.AddPoints(spawnedBuilding.GetPoints().amount, spawnedBuilding.GetPoints().pointType, PointManager.EntityType.Player);

        UpdateButtons();

        if (!buildings[currentIndex].multiPlace)
        {
            ResetObjectServerRpc();
        }
    }

    public void SelectObject(int index)
    {
        ResetObjectServerRpc();
        pendingObject = Instantiate(buildings[index].building, pos.Value, transform.rotation);


        ChangeObjectMaterial(pendingObject, correctPlaceMaterial);

        currentIndex = index;
    }




    private void ChangeObjectMaterial(GameObject go, Material material)
    {
        if (go.TryGetComponent(out MeshRenderer mr))
        {
            if (mr.material)
            {
                mr.material = material;
            }
        }
        else
        {
            foreach (var mr2 in go.GetComponentsInChildren<MeshRenderer>())
            {
                Material[] materials = mr2.materials;

                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = material;
                }

                mr2.materials = materials;
            }
        }
    }

    public bool GetOccupany(GameObject building)
    {
        building.layer = (int)Mathf.Log(tempBuildingLayerMask.value, 2);
        Transform trans = building.transform;

        Collider collider = building.GetComponent<Collider>();

        Collider[] colliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, trans.rotation, buildLayerMask);
        if (colliders.Length > 0)
        {
            return true;
        }

        return false;
    }
}