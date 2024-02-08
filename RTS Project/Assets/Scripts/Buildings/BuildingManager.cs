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
    [SerializeField] private float minHeight;

    public GameObject pendingObject;
    private int currentIndex = -1;
    private Vector3 pos;
    private RaycastHit hit;
    private bool rayHit;

    private float currentDegreesRotated;

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

    private void UpdateButtons()
    {
        for (int i = 0; i < buildings.Length; i++)
        {
            Button button = buildings[i].button;

            if (button == null)
            {
                Debug.LogError("Building button not found");
                continue;
            }
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
        if (currentIndex < 0) return;
        if (!pendingObject) return;

        pendingObject.transform.position = pos;

        //Change pending object material based on if it can be placed or not
        if (!CheckCanPlace(false))
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
            pos = gridPos;

            //rotate object towards hit.normal
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            Vector3 eulerAngles = rotation.eulerAngles;

            eulerAngles.y += currentDegreesRotated;

            rotation.eulerAngles = eulerAngles;

            pendingObject.transform.rotation = rotation;
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
            ResetObject();
        }
        //Rotate pending object
        else if (Input.GetKeyDown(KeyCode.R))
        {
            //Reverse rotation when holding leftshift
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentDegreesRotated -= degreesToRotate;
            }
            else
            {
                currentDegreesRotated += degreesToRotate;
            }
        }
        //place object
        else if (Input.GetMouseButtonDown(0))
        {
            if (CheckCanPlace(true))
            {
                BuildObject();
            }
        }
    }

    private bool CheckCanPlace(bool spawnError)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return false;

        if (!rayHit) return false;
        float rayAngle = Vector3.Angle(pendingObject.transform.up, Vector3.up);

        if (rayAngle > maxAngle)
        {
            if (spawnError)
            {
                SpawnError("angle too steep");
            }

            return false;
        }

        if (pendingObject.transform.position.y > maxHeight)
        {
            if (spawnError)
            {
                SpawnError("Too high");
            }

            return false;
        }

        if (pendingObject.transform.position.y < minHeight)
        {
            if (spawnError)
            {
                SpawnError("Too low");
            }

            return false;
        }

        pendingObject.SetActive(true);
        //check collision
        if (GetOccupany(pendingObject) || !rayHit)
        {
            if (spawnError)
            {
                SpawnError("Can't place building here");
            }
            return false;
        }

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
                    if (spawnError)
                    {
                        SpawnError($"needs {itemNeeded.amountNeeded - resources.GetSlotByItemData(itemNeeded.data).amount} " +
                            $"more : {itemNeeded.data.name}");
                    }
                    hasEverything = false;
                    return false;
                }
            }
        }

        //remove items only when the player can actually build it
        if (hasEverything)
        {
            savedSlots.Clear();

            //build object
            return true;
        }

        if (spawnError)
        {
            SpawnError("Other error");
        }
        return false;
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

    private void ResetObject()
    {
        Destroy(pendingObject);
        pendingObject = null;
        currentIndex = -1;
        pos = Vector3.zero;
    }

    private void BuildObject()
    {
        foreach (var itemNeeded in buildings[currentIndex].building.GetComponent<BuildingBase>().GetRecipes())
        {
            if (itemNeeded.data == resources.GetSlotByItemData(itemNeeded.data).data)
            {
                resources.GetSlotByItemData(itemNeeded.data).amount -= itemNeeded.amountNeeded;
            }
        }

        ParticleSystem spawnedParticle = Instantiate(buildParticle, pos, Quaternion.identity).GetComponent<ParticleSystem>();
        spawnedParticle.Play();

        BuildingBase spawnedBuilding = Instantiate(buildings[currentIndex].building, pendingObject.transform.position, 
            pendingObject.transform.rotation).GetComponent<BuildingBase>();

        spawnedBuilding.Init(buildingMaterial, buildParticle, 
            buildings[currentIndex].building, buildings[currentIndex].buildTime, BuildingBase.States.Building);

        spawnedBuilding.SetOccupancyType(BuildingBase.OccupancyType.Player);
        spawnedBuilding.Build();

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
            ResetObject();
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

        Collider collider = building.GetComponent<Collider>();

        Collider[] colliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, trans.rotation, buildLayerMask);

        if (colliders.Length > 0)
        {
            print("collided with something. Count : " + colliders[0].name);
            return true;
        }

        return false;
    }
}