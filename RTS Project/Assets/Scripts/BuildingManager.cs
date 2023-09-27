using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlaceableObject[] objects;
    [SerializeField] private ResourceManager resources;

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

    private GameObject pendingObject;
    private GameObject tempObject;
    private int currentIndex = -1;
    private Vector3 pos;
    private RaycastHit hit;
    private bool rayHit;

    [System.Serializable]
    class PlaceableObject
    {
        public GameObject model;
        public float yHeight;
        public Recipe recipe;
        public bool multiPlace;
        public Gradient buildParticleRandomColor;
        public float buildTime;
    }

    [System.Serializable]
    class Recipe
    {
        public RecipeItem[] items;
    }

    [System.Serializable]
    class RecipeItem
    {
        public ItemData data;
        public int amountNeeded;
    }

    void Update()
    {
        if (!pendingObject) return;

        pendingObject.transform.position = pos;

        //Change pending object material based on if it can be placed or not
        if (GridManager.Instance.GetOccupanyPendingObject())
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
        //check collision
        if (!GridManager.Instance.GetOccupanyPendingObject() && rayHit)
        {
            bool hasEverything = true;

            List<ItemSlot> savedSlots = new();

            //loop through all recipe items and all resources and check if the player has enough resources to build the building
            foreach (var itemNeeded in objects[currentIndex].recipe.items)
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
                foreach (var itemNeeded in objects[currentIndex].recipe.items)
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
                StartCoroutine(BuildObject());
            }
        }
        else
        {
            SpawnError("Can't place building here");
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

    IEnumerator BuildObject()
    {
        float randomNum = Random.Range(0f, 1f);
        buildParticleMaterial.color = objects[currentIndex].buildParticleRandomColor.Evaluate(randomNum);
        buildParticle.GetComponent<ParticleSystemRenderer>().material = buildParticleMaterial;

        ParticleSystem spawnedParticle = Instantiate(buildParticle, pos, Quaternion.identity).GetComponent<ParticleSystem>();
        spawnedParticle.Play();

        int saveCurrentIndex = currentIndex;
        GameObject savePendingObject = pendingObject;

        if (!objects[currentIndex].multiPlace)
        {
            ResetObject();
        }

        GridManager.Instance.CheckOccupancy();

        ChangeObjectMaterial(savePendingObject, buildingMaterial);
        tempObject = Instantiate(savePendingObject, savePendingObject.transform.position, savePendingObject.transform.rotation);

        yield return new WaitForSeconds(objects[saveCurrentIndex].buildTime);

        Instantiate(objects[saveCurrentIndex].model, tempObject.transform.position, tempObject.transform.rotation);
        Destroy(tempObject);
        yield return null;
    }

    private void FixedUpdate()
    {
        if (currentIndex < 0) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000, buildLayerMask))
        {
            rayHit = true;
            pos = new Vector3(GridManager.Instance.GetClosestPointOnGrid(hit.point).x, objects[currentIndex].yHeight, GridManager.Instance.GetClosestPointOnGrid(hit.point).z);
        }
        else
        {
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
                List<Material> materials = new();

                for (int i = 0; i < mr2.materials.Length; i++)
                {
                    materials.Add(material);
                }

                mr2.SetMaterials(materials);
            }
        }
    }
}