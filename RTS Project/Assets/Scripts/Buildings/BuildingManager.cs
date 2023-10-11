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
    [SerializeField] private PlaceableObject[] objects;
    [SerializeField] private ResourceItemManager resources;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Terrain terrain;

    [Header("Build Progresses")]
    [SerializeField] private GameObject buildProgressPrefab;
    [SerializeField] private float buildProgressHeight;

    [Header("Particle")]
    [SerializeField] private GameObject buildParticle;
    [SerializeField] private Material buildParticleMaterial;

    [Header("Materials")]
    [SerializeField] private Material buildingMaterial;
    [SerializeField] private Material correctPlaceMaterial;
    [SerializeField] private Material incorrectPlaceMaterial;

    [Header("Build Errors")]
    [SerializeField] private GameObject buildErrorPrefab;
    [SerializeField] private Transform buildErrorParent;

    [Header("Variables")]
    [SerializeField] private LayerMask buildLayerMask;
    [SerializeField] private float degreesToRotate = 90f;
    [SerializeField] private float buildErrorFloatUpSpeed;
    [SerializeField] private Button[] buttons;

    private GameObject pendingObject;
    private int currentIndex = -1;
    private Vector3 pos;
    private RaycastHit hit;
    private bool rayHit;

    [System.Serializable]
    class PlaceableObject
    {
        public GameObject model;
        public float yHeight;
        public Recipe[] recipes;
        public bool multiPlace;
        public Gradient buildParticleRandomColor;
        public float buildTime;
        public bool isUnlocked;
        public int[] buildingsToUnlock;
    }

    private void Awake()
    {
        buildParticleMaterial.color = Color.white;
    }

    private void Start()
    {
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            buttons[i].interactable = false;

            if (objects[i].isUnlocked)
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
        if (gridManager.GetOccupany(pos, pendingObject))
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
            pendingObject.SetActive(true);
            //check collision
            if (!gridManager.GetOccupany(pos, pendingObject) && rayHit)
            {
                bool hasEverything = true;

                List<ItemSlot> savedSlots = new();

                //loop through all recipe items and all resources and check if the player has enough resources to build the building
                foreach (var itemNeeded in objects[currentIndex].recipes)
                {
                    foreach (var itemGot in resources.GetAllResources())
                    {
                        if (itemNeeded.data == itemGot.data)
                        {
                            if (itemGot.amount >= itemNeeded.amountNeeded)
                            {
                                savedSlots.Add(itemGot);
                                hasEverything = true;
                            }
                            else
                            {
                                SpawnError($"needs {itemNeeded.amountNeeded - itemGot.amount} more : {itemNeeded.data.name}");
                                hasEverything = false;
                            }
                        }
                    }
                }

                //remove items only when the player can actually build it
                if (hasEverything)
                {
                    foreach (var itemNeeded in objects[currentIndex].recipes)
                    {
                        foreach (var itemGot in resources.GetAllResources())
                        {
                            if (itemNeeded.data == itemGot.data)
                            {
                                itemGot.amount -= itemNeeded.amountNeeded;
                            }
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
        float randomNum = Random.Range(0f, 1f);
        buildParticleMaterial.color = objects[currentIndex].buildParticleRandomColor.Evaluate(randomNum);
        buildParticle.GetComponent<ParticleSystemRenderer>().material = buildParticleMaterial;

        ParticleSystem spawnedParticle = Instantiate(buildParticle, pos, Quaternion.identity).GetComponent<ParticleSystem>();
        spawnedParticle.Play();

        BuildingBase spawnedBuilding = Instantiate(objects[currentIndex].model, pendingObject.transform.position, pendingObject.transform.rotation).GetComponent<BuildingBase>();
        spawnedBuilding.Init(buildingMaterial, buildParticle);
        StartCoroutine(spawnedBuilding.Build(objects[currentIndex].buildTime));

        BuildProgress buildProgress = Instantiate(buildProgressPrefab, new Vector3(spawnedBuilding.transform.position.x,
            spawnedBuilding.transform.position.y + spawnedBuilding.transform.localScale.y + buildProgressHeight, spawnedBuilding.transform.position.z), Quaternion.identity).GetComponent<BuildProgress>();
        buildProgress.Init(objects[currentIndex].buildTime);

        for (int i = 0; i < objects[currentIndex].buildingsToUnlock.Length; i++)
        {
            objects[objects[currentIndex].buildingsToUnlock[i]].isUnlocked = true;
        }

        UpdateButtons();

        if (!objects[currentIndex].multiPlace)
        {
            ResetObject();
        }

        gridManager.SetOccupancy(spawnedBuilding.transform.position);
    }

    private void FixedUpdate()
    {
        if (currentIndex < 0) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000, buildLayerMask))
        {
            pendingObject.SetActive(true);
            rayHit = true;
            //Vector3 gridPos = gridManager.GetClosestPointOnGrid(hit.point);

            //check raycast for terrain hit normal and check if can place

            Vector3 gridPos = Vector3Int.RoundToInt(hit.point);
            if (terrain)
            {
                gridPos.y = terrain.SampleHeight(gridPos) + objects[currentIndex].model.transform.localScale.y;
            }
            else
            {
                gridPos.y = objects[currentIndex].model.transform.localScale.y;
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

        pendingObject = Instantiate(objects[index].model, pos, transform.rotation);

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
}