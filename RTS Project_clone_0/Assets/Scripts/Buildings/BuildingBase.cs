using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingBase : MonoBehaviour
{
    [SerializeField] public float buildingHp = 50f;
    [SerializeField] protected States currentState;
    [SerializeField] private Recipe[] recipes;
    [SerializeField] private BuildingPoints points;
    [SerializeField] private Outline outline;
    [SerializeField] private GameObject model;

    public string buildingName;

    private List<Material> savedMaterials = new();
    private GameObject particleObject;

    private Material buildingMaterial;

    private UIManager uiManager;

    private float outlineDefaultSize;

    private GameObject buildingToSpawn;

    private float buildingAnimationValue;

    public enum Jobs { Wood, Stone, Metal }

    [SerializeField] private OccupancyType occupancyType;

    public enum States
    {
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

    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();

        outline.enabled = false;
        outline.OutlineWidth = uiManager.GetOutlineDefaultSize();
        outlineDefaultSize = outline.OutlineWidth;

        if (buildingName == string.Empty)
            buildingName = name;
    }

    public virtual void Init(Material _material, GameObject _particleObject, GameObject buildingToSpawn, States state)
    {
        buildingMaterial = _material;
        particleObject = _particleObject;
        this.buildingToSpawn = buildingToSpawn;
        currentState = state;

        if (buildingMaterial)
            buildingAnimationValue = buildingMaterial.GetFloat("_Min");
    }

    public virtual IEnumerator Build(float buildTime)
    {
        if (currentState == States.Normal) yield return null;

        ChangeObjectMaterial(buildingMaterial);

        float max = buildingMaterial.GetFloat("_Max");
        float min = buildingMaterial.GetFloat("_Min");
        float range = max - min;
        buildTime *= 250;
        float speed = range / buildTime;

        while (buildingAnimationValue < max)
        {
            buildingAnimationValue += speed;
            buildingMaterial.SetFloat("_Value", buildingAnimationValue);
            yield return null;
        }

        currentState = States.Normal;
        ParticleSystem particle = Instantiate(particleObject, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        particle.Play();
        yield return new WaitForSeconds(particle.main.duration);

        Instantiate(buildingToSpawn, transform.position, transform.rotation).TryGetComponent(out BuildingBase spawnedBuilding);
        spawnedBuilding.Init(null, null, null, States.Normal);

        yield return null;

        Destroy(gameObject);
    }

    public States GetCurrentState() => currentState;

    private void ChangeObjectMaterial(Material material)
    {
        if (model.TryGetComponent(out MeshRenderer mesh))
        {
            if (mesh.material)
            {
                mesh.material = material;
            }
        }
        else
        {
            foreach (var mr in model.GetComponentsInChildren<MeshRenderer>())
            {
                Material[] materials = mr.materials;

                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = material;
                }

                mr.materials = materials;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            DestroyBuilding();
        }
    }

    public virtual void SelectBuilding()
    {
        if (currentState == States.Building) return;

        uiManager.SetBuildingUI(true, this);
        outline.OutlineWidth = outlineDefaultSize;
        outline.enabled = true;
    }

    public virtual void DeselectBuilding()
    {
        if (currentState == States.Building) return;

        uiManager.SetBuildingUI(false, this);
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