using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingBase : NetworkBehaviour
{
    [SerializeField] public float buildingHp = 50f;
    [SerializeField] protected States currentState;
    [SerializeField] private Recipe[] recipes;
    [SerializeField] private BuildingPoints points;
    [SerializeField] private Outline outline;
    [SerializeField] private GameObject model;

    public string buildingName;

    private GameObject particleObject;

    private Material buildingMaterial;

    private UIManager uiManager;

    private float outlineDefaultSize;

    private GameObject buildingToSpawn;

    private float buildingAnimationValue;

    public enum Jobs { Wood, Stone, Metal }

    [SerializeField] private OccupancyType occupancyType;

    List<Material[]> savedMaterials = new List<Material[]>();
    List<MeshRenderer> savedRenderers = new List<MeshRenderer>();


    private float minBuildValue;
    private float maxBuildValue;
    private float buildSpeed;

    private float particleTimer;
    private bool spawnedParticle;

    public enum States
    {
        Pending,
        Building,
        Normal
    }

    public enum OccupancyType
    {
        None,
        Player,
        Enemy
    }

    #region Occupancy

    public void SetOccupancyType(OccupancyType occupancyType)
    {
        this.occupancyType = occupancyType;
    }

    public OccupancyType GetOccupancyType()
    {
        return occupancyType;
    }

    #endregion

    #region classes

    [System.Serializable]
    public class BuildingPoints
    {
        public PointManager.PointType pointType;
        public int amount;
    }

    public BuildingPoints GetPoints()
    {
        return points;
    }

    public Recipe[] GetRecipes()
    {
        return recipes;
    }

    #endregion

    protected virtual void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        //print("UI manager : " + uiManager + " from " + name);

        if (outline)
        {
            outline.enabled = false;
            outline.OutlineWidth = uiManager.GetOutlineDefaultSize();
            outlineDefaultSize = outline.OutlineWidth;
        }

        if (buildingName == string.Empty)
            buildingName = name;
    }

    [ClientRpc]
    public virtual void InitClientRpc(float buildTime, States state)
    {
        print("initserverrpc called");
        buildingToSpawn = gameObject;
        currentState = state;


        if (currentState == States.Building)
        {
            StoreRenderersInChildren();
            Material _material = FindAnyObjectByType<BuildingManager>().buildingMaterial;
            GameObject _particleObject = FindAnyObjectByType<BuildingManager>().buildParticle;

            Material newMaterial = new(_material.shader)
            {
                color = _material.color
            };

            buildingMaterial = newMaterial;

            particleObject = _particleObject;

            if (buildingMaterial)
            {
                buildingAnimationValue = buildingMaterial.GetFloat("_Min");

                maxBuildValue = buildingMaterial.GetFloat("_Max");
                minBuildValue = buildingMaterial.GetFloat("_Min");
            }

            float range = maxBuildValue - minBuildValue;
            buildTime *= 250;
            buildSpeed = range / buildTime;

            ChangeObjectMaterial(buildingMaterial);

        }
        if (currentState == States.Normal) 
        {
            ApplyMaterialsToStoredRenderers();
            //buildingMaterial = null;    
            //ChangeObjectMaterial(buildingMaterial);

        }



        if (buildingMaterial)
            buildingAnimationValue = buildingMaterial.GetFloat("_Min");
    }

    void StoreRenderersInChildren()
    {
        // Iterate through all child objects
        foreach (Transform child in model.transform)
        {
            print("store");
            // Get the MeshRenderer component of the child
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();

            if (renderer != null)
            {
                // Add the MeshRenderer to the list
                savedRenderers.Add(renderer);

                // Get the shared materials of the MeshRenderer and create a copy
                Material[] materials = renderer.sharedMaterials;
                Material[] materialsCopy = new Material[materials.Length];
                System.Array.Copy(materials, materialsCopy, materials.Length);

                // Add the materials copy to the list
                savedMaterials.Add(materialsCopy);
            }
        }
    }

    void ApplyMaterialsToStoredRenderers()
    {
        // Iterate through all stored MeshRenderers and their corresponding materials
        for (int i = 0; i < savedRenderers.Count; i++)
        {
            print("apply");

            // Apply the stored materials to the MeshRenderer
            savedRenderers[i].sharedMaterials = savedMaterials[i];
        }
    }
    [ClientRpc]
    public virtual void BuildClientRpc()
    {
        if (currentState == States.Normal || currentState == States.Pending) return;

        if (buildingAnimationValue < maxBuildValue)
        {
            buildingAnimationValue += buildSpeed;
            buildingMaterial.SetFloat("_Value", buildingAnimationValue);
        }
        else
        {
            if (!spawnedParticle)
            {
                ParticleSystem particle = Instantiate(particleObject, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
                particle.Play();
                particleTimer = particle.main.duration;
                spawnedParticle = true;
            }
            else
            {
                particleTimer -= Time.deltaTime;

                if (particleTimer < 0)
                {
                    InitClientRpc(0, States.Normal);

                }
            }
        }
    }
    
    public States GetCurrentState() => currentState;

    private void ChangeObjectMaterial(Material material)
    {
        if (!model) return;

        if (model.TryGetComponent(out MeshRenderer mesh))
        {
            if (mesh.material)
            {
                mesh.material = material;
            }
        }
        else
        {
            foreach (Transform child in model.transform)
            {
                if (child.TryGetComponent(out MeshRenderer mesh2))
                {
                    Material[] materials = mesh2.materials;

                    for (int i = 0; i < materials.Length; i++)
                    {
                        materials[i] = material;
                    }

                    mesh2.materials = materials;
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            DestroyBuilding();
        }
        BuildClientRpc();
    }

    public virtual void SelectBuilding()
    {
        if (currentState == States.Building) return;

        if (uiManager)
            uiManager.SetBuildingUI(true, this);

        if (outline)
        {
            outline.OutlineWidth = outlineDefaultSize;
            outline.enabled = true;
        }
    }

    public virtual void DeselectBuilding()
    {
        if (currentState == States.Building) return;

        if (uiManager)
            uiManager.SetBuildingUI(false, this);

        if (outline)
            outline.enabled = false;
    }

    public virtual void DestroyBuilding()
    {
        if (currentState == States.Building) return;

        uiManager.SetBuildingUI(false, this);
        Destroy(gameObject);
    }

    public Outline GetOutline() => outline;

    public void StartAnimateOutline()
    {
        hasGrown = false;
        isDone = false;
        outline.OutlineWidth = 0f;
        outline.enabled = true;

        StartCoroutine(AnimateOutline());
    }

    private bool hasGrown;
    private bool isDone;

    private IEnumerator AnimateOutline()
    {
        while (!isDone)
        {
            if (!hasGrown)
            {
                while (outline.OutlineWidth < uiManager.GetOutlineAnimationMaxSize())
                {
                    outline.OutlineWidth += 0.1f;

                    if (outline.OutlineWidth >= uiManager.GetOutlineAnimationMaxSize())
                    {
                        hasGrown = true;
                        yield return new WaitForSeconds(uiManager.GetOutlineAnimationFinishedWaitTime());
                    }

                    yield return new WaitForSeconds(uiManager.GetOutlineAnimationSpeed());
                }
            }
            else
            {
                while (outline.OutlineWidth > 0f)
                {
                    outline.OutlineWidth -= 0.1f;

                    if (outline.OutlineWidth <= 0f)
                    {
                        isDone = true;
                    }

                    yield return new WaitForSeconds(uiManager.GetOutlineAnimationSpeed());
                }
            }
        }

        outline.enabled = false;

        yield return null;
    }
}