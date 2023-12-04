using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    // Naming and other basic variables
    [Header("Unit variables")]
    [SerializeField] protected int unitHealth;
    [SerializeField] protected int unitMaxHealth;
    [SerializeField] protected int unitHealing;
    [SerializeField] protected string unitName;
    
    public int UnitHealth { get { return unitHealth; } set { unitHealth = value; } }

    public int UnitMaxHealth { get { return unitMaxHealth; } set { UnitMaxHealth = value; } }

    public string UnitName { get { return unitName; } set { unitName = value; } }

    [Space]
    [SerializeField] protected int unitSpeed;
    [SerializeField] protected int unitDamage;

    [Header("Inventory Stuff")]
    protected int maxItems = 5;
    protected int[] inventorySlots = new int[3];

    [Header("Enum Data")]
    [SerializeField] protected Jobs job;
    [SerializeField] public TypeUnit typeUnit;
    [SerializeField] private Sex sex;

    [Header("Select Agent Movement")]
    [SerializeField] GameObject selectionObject;
    [SerializeField] protected NavMeshAgent myAgent;
    [SerializeField] LayerMask groundLayer;
    [Space]
    [SerializeField] GameObject marker;
    [SerializeField] LayerMask clickableUnit;
    [SerializeField] protected Color selectionColor;
    [SerializeField] private int cameraResoltion = 64;
    [SerializeField] private float cameraFPS = 5;

    private Camera unitCamera;
    private RenderTexture renderTexture;

    private bool isSelected;

    private string currentAction;

    private void Awake()
    {
        unitCamera = GetComponentInChildren<Camera>();
    }

    protected virtual void Start()
    {
        selectionObject.SetActive(false);
        selectionObject.GetComponent<MeshRenderer>().material.color = selectionColor;

        unitCamera.gameObject.SetActive(false);
        unitCamera.enabled = false;

        RandomSex();

        RandomName();
    }

    private void RandomName()
    {
        TextAsset file = null;

        if (sex == Sex.Female)
        {
            file = Resources.Load<TextAsset>("FemaleNames");
        }
        else if (sex == Sex.Male)
        {
            file = Resources.Load<TextAsset>("MaleNames");
        }

        string[] names = file.text.Split('\n');
        int randomNum = Random.Range(0, names.Length);
        UnitName = names[randomNum];
    }

    private void RandomSex()
    {
        var values = System.Enum.GetValues(typeof(Sex));
        int randomNum = Random.Range(0, values.Length);
        sex = (Sex)values.GetValue(randomNum);
    }

    public void Select()
    {
        if (isSelected) return;

        //renderTexture = new(cameraResoltion, cameraResoltion, 0)
        //{
        //    name = gameObject.name + " Render Texture"
        //};
        //unitCamera.targetTexture = renderTexture;
        //unitCamera.gameObject.SetActive(true);
        isSelected = true;
    }

    public void Deselect()
    {
        //Destroy(renderTexture);
        //unitCamera.targetTexture = null;
        //unitCamera.gameObject.SetActive(false);
        isSelected = false;
    }

    float elapsed = 0;

    protected virtual void Update()
    {
        if (isSelected)
        {
            elapsed += Time.deltaTime;
            if (elapsed > 1 / cameraFPS)
            {
                elapsed = 0;
                unitCamera.Render();
            }
        }
    }

    #region Enums

    protected enum Jobs
    {
        None,
        Builder,
        Hunter,
        Explorer,
        Miner,
        Nurse
    }

    public enum TypeUnit
    {
        None,
        Human,
        Animal,
        Enemy,
        Special
    }

    protected enum Sex
    {
        Female,
        Male
    }

    #endregion

    #region Combat Functions

    protected virtual int TakeDamage(int dealtDamage)
    {
        int remainingHealth = this.unitHealth - dealtDamage;
        return remainingHealth;
    }

    protected virtual int DealDamage(int damage)
    {
        return damage;
    }

    protected virtual void Death()
    {

    }

    protected virtual int Heal(int healing)
    {
        int gainedHealth = unitHealth + healing;

        return gainedHealth;
    }

    #endregion

    #region Unit Location Controller

    public void SetSelectionObject(bool value) => selectionObject.SetActive(value);

    public void SendUnitToLocation(Vector3 pos)
    {
        //doesnt work entirely
        if (Vector3.Distance(transform.position, pos) > 1)
        {
            myAgent.SetDestination(pos);
        }
        else
        {
            myAgent.SetDestination(transform.position);
        }
    }

    #endregion

    public RenderTexture GetRenderTexture() => renderTexture;

    public string GetCurrentAction() => currentAction;

    public void SetCurrentAction(string currentAction) => this.currentAction = currentAction;
}
