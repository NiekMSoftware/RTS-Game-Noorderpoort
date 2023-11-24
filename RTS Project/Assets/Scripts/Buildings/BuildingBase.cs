using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBase : MonoBehaviour
{
    [SerializeField] public float buildingHp = 50f;
    [SerializeField] private States currentState;
    [SerializeField] private Recipe[] recipes;
    [SerializeField] private BuildingPoints points;
    [SerializeField] private Outline outline;

    public string buildingName;

    private List<Material> savedMaterials = new();
    private GameObject particleObject;

    private Material buildingMaterial;

    private UIManager uiManager;

    private float outlineDefaultSize;

    public enum Jobs { Wood, Stone, Metal }

    private void Awake()
    {
        outline = GetComponent<Outline>();

        uiManager = FindObjectOfType<UIManager>();

        outline.enabled = false;
        outline.OutlineWidth = uiManager.GetOutlineDefaultSize();
        outlineDefaultSize = outline.OutlineWidth;

        if (buildingName == string.Empty)
            buildingName = name;
    }

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

    public void SetOccupancyType(OccupancyType occupancyType)
    {
        this.occupancyType = occupancyType;
    }

    public OccupancyType GetOccupancyType()
    {
        return occupancyType;
    }

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

    public virtual void Init(Material _material, GameObject _particleObject)
    {
        buildingMaterial = _material;
        particleObject = _particleObject;
    }

    public virtual IEnumerator Build(float buildTime)
    {
        currentState = States.Building;

        SaveObjectMaterials();
        ApplyObjectMaterials();
        //ChangeObjectMaterial(buildingMaterial);
        yield return new WaitForSeconds(buildTime);

        currentState = States.Normal;
        ParticleSystem particle = Instantiate(particleObject, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        particle.Play();
        yield return new WaitForSeconds(particle.main.duration);

        yield return null;
    }

    public States GetCurrentState() => currentState;

    private void ChangeObjectMaterial(Material material)
    {
        if (gameObject.TryGetComponent(out MeshRenderer mr))
        {
            if (mr.material)
            {
                mr.material = material;
            }
        }
        else
        {
            foreach (var mr2 in gameObject.GetComponentsInChildren<MeshRenderer>())
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            DestroyBuilding();
        }
    }

    public virtual void SelectBuilding()
    {
        print("select building!");
        uiManager.SetBuildingUI(true, this);
        outline.OutlineWidth = outlineDefaultSize;
        outline.enabled = true;
    }

    public virtual void DeselectBuilding()
    {
        uiManager.SetBuildingUI(false, this);
        outline.enabled = false;
    }

    public virtual void DestroyBuilding()
    {
        uiManager.SetBuildingUI(false, this);
        Destroy(gameObject);
    }

    private void ApplyObjectMaterials()
    {
        if (gameObject.TryGetComponent(out MeshRenderer mr))
        {
            if (mr.material)
            {
                mr.material = savedMaterials[0];
            }
        }
        else
        {
            foreach (var mr2 in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                Material[] materials = mr2.materials;

                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = savedMaterials[i];
                }

                mr2.materials = materials;
            }
        }
    }

    private void SaveObjectMaterials()
    {
        if (gameObject.TryGetComponent(out MeshRenderer mr))
        {
            if (mr.material)
            {
                savedMaterials[0] = mr.material;
            }
        }
        else
        {
            foreach (var mr2 in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                Material[] materials = mr2.materials;

                for (int i = 0; i < materials.Length; i++)
                {
                    //print(materials[i].name + " " + i + mr2.transform.GetSiblingIndex());
                    savedMaterials.Add(materials[i]);
                }
            }
        }
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