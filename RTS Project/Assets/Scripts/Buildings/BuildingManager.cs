using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Building[] buildings;
    [SerializeField] private ResourceItemManager resources;
    [SerializeField] private Terrain terrain;
    [SerializeField] private Button[] buttons;
    [SerializeField] private PointManager pointManager;

    [Header("Build Progresses")]
    [SerializeField] private GameObject buildProgressPrefab;
    [SerializeField] private float buildProgressHeight;

    [Header("Particle")]
    [SerializeField] private GameObject buildParticle;

    [Header("Materials")]
    [SerializeField] private Material buildingMaterial;
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

    private GameObject pendingObject;
    private int currentIndex = -1;
    private Vector3 pos;
    private RaycastHit hit;
    private bool rayHit;

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
    }

    private void Start()
    {
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        for (int i = 0; i < buildings.Length; i++)
        {
            buttons[i].interactable = false;

            if (buildings[i].isUnlocked)
            {
                buttons[i].interactable = true;
            }
        }
    }

    void Update()
    {
        if (!pendingObject) return;

        pendingObject.transform.position = pos;

        //Change pending object material based on if it can be placed or not
        if (GetOccupany(pendingObject))
        {
            ChangeObjectMaterial(pendingObject, incorrectPlaceMaterial);
        }
        else
        {
            ChangeObjectMaterial(pendingObject, correctPlaceMaterial);
        }

        HandleInput();
    }

    private void HandleInput()
    {
        //Destroy and reset pending object
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetObject();
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
        else if (Input.GetMouseButtonDown(0))
        {
            CheckCanPlace();
        }
    }

    private void CheckCanPlace()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (rayHit)
        {
            float rayAngle = Vector3.Angle(pendingObject.transform.forward, hit.normal);

            if (rayAngle <= maxAngle)
            {
                if (pendingObject.transform.position.y <= maxHeight)
                {
                    pendingObject.SetActive(true);
                    //check collision
                    if (!GetOccupany(pendingObject) && rayHit)
                    {
                        bool hasEverything = true;

                        List<ItemSlot> savedSlots = new();

                        //loop through all recipe items and all resources and check if the player has enough resources to build the building
                        foreach (var itemNeeded in buildings[currentIndex].building.GetComponent<BuildingBase>().GetRecipes())
                        {
                            if (itemNeeded.data == resources.GetSlotByItemData(itemNeeded.data).data)
                            {
                                if (resources.GetSlotByItemData(itemNeeded.data).amount >= itemNeeded.amountNeeded)
                                {
                                    savedSlots.Add(resources.GetSlotByItemData(itemNeeded.data));
                                    hasEverything = true;
                                }
                                else
                                {
                                    SpawnError($"needs {itemNeeded.amountNeeded - resources.GetSlotByItemData(itemNeeded.data).amount} more : {itemNeeded.data.name}");
                                    hasEverything = false;
                                }
                            }
                        }

                        //remove items only when the player can actually build it
                        if (hasEverything)
                        {
                            foreach (var itemNeeded in buildings[currentIndex].building.GetComponent<BuildingBase>().GetRecipes())
                            {
                                if (itemNeeded.data == resources.GetSlotByItemData(itemNeeded.data).data)
                                {
                                    resources.GetSlotByItemData(itemNeeded.data).amount -= itemNeeded.amountNeeded;
                                }
                            }

                            savedSlots.Clear();

                            //build object
                            BuildObject();
                        }
                    }
                    else
                    {
                        SpawnError("Can't place building here");
                    }
                }
                else
                {
                    SpawnError("Too high");
                }
            }
            else
            {
                SpawnError("angle too steep");
            }
        }
    }

    public GameObject GetPendingObject() => pendingObject;

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

    private void ResetObject()
    {
        Destroy(pendingObject);
        pendingObject = null;
        currentIndex = -1;
        pos = Vector3.zero;
    }

    private void BuildObject()
    {
        ParticleSystem spawnedParticle = Instantiate(buildParticle, pos, Quaternion.identity).GetComponent<ParticleSystem>();
        spawnedParticle.Play();

        BuildingBase spawnedBuilding = Instantiate(buildings[currentIndex].building, pendingObject.transform.position, pendingObject.transform.rotation).GetComponent<BuildingBase>();
        spawnedBuilding.Init(buildingMaterial, buildParticle);
        spawnedBuilding.SetOccupancyType(BuildingBase.OccupancyType.Player);
        StartCoroutine(spawnedBuilding.Build(buildings[currentIndex].buildTime));

        BuildProgress buildProgress = Instantiate(buildProgressPrefab, new Vector3(spawnedBuilding.transform.position.x,
            spawnedBuilding.transform.position.y + spawnedBuilding.transform.localScale.y + buildProgressHeight, spawnedBuilding.transform.position.z), Quaternion.identity).GetComponent<BuildProgress>();
        buildProgress.Init(buildings[currentIndex].buildTime);

        for (int i = 0; i < buildings[currentIndex].buildingsToUnlock.Length; i++)
        {
            buildings[buildings[currentIndex].buildingsToUnlock[i]].isUnlocked = true;
        }

        pointManager.AddPoints(spawnedBuilding.GetPoints().amount, spawnedBuilding.GetPoints().pointType, PointManager.EntityType.Player);

        UpdateButtons();

        if (!buildings[currentIndex].multiPlace)
        {
            ResetObject();
        }
    }

    private void FixedUpdate()
    {
        if (currentIndex < 0) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000, groundLayerMask))
        {
            pendingObject.SetActive(true);
            rayHit = true;

            //check raycast for terrain hit normal and check if can place

            Vector3 gridPos = Vector3Int.RoundToInt(hit.point);
            if (terrain)
            {
                gridPos.y = terrain.SampleHeight(gridPos) + buildings[currentIndex].building.transform.localScale.y;
            }
            else
            {
                gridPos.y = buildings[currentIndex].building.transform.localScale.y;
            }
            pos = gridPos;

            //rotate object towards hit.normal
        }
        else
        {
            pendingObject.SetActive(false);
            rayHit = false;
        }
    }

    public void SelectObject(int index)
    {
        ResetObject();

        pendingObject = Instantiate(buildings[index].building, pos, transform.rotation);

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

        //Collider[] colliders = null;
        Collider[] colliders = Physics.OverlapBox(trans.position, trans.localScale / 2, trans.rotation, buildLayerMask);

        if (colliders.Length > 0)
        {
            return true;
        }
        //if (colliders[0] != null)
        //{
        //    return true;
        //}

        return false;
    }
}