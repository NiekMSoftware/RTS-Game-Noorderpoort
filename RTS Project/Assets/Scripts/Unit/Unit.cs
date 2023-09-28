using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    // Naming and other basic variables
    [Header("Unit variables")]
    [SerializeField] protected int unitHealth;
    [SerializeField] protected int unitMaxHealth;
    [SerializeField] protected int unitHealing;

    [Space]
    [SerializeField] protected int unitSpeed;
    [SerializeField] protected int unitDamage;

    [Header("Inventory Stuff")]
    protected int maxItems = 5;
    protected int[] inventorySlots = new int[3];

    [Header("Enum Data")]
    [SerializeField] protected Jobs job;
    [SerializeField] protected TypeUnit typeUnit;

    [Header("Select Agent Movement")]
    [SerializeField] GameObject selectionObject;
    [SerializeField] NavMeshAgent myAgent;
    [SerializeField] LayerMask groundLayer;
    [Space]
    [SerializeField] GameObject marker;
    [SerializeField] LayerMask clickableUnit;

    SelectUnits mySelectionUnits;
    Camera myCamera;

    //void Start()
    //{
    //    InitSelection();
    //}

    //void Update()
    //{
    //    SendUnitToLocation();
    //    CustomSelectionUnits();
    //}

    #region Enums

    public enum Jobs
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
        DarkElves
    }

    #endregion

    #region Combat Functions

    public int TakeDamage(int dealtDamage)
    {
        int remainingHealth = this.unitHealth - dealtDamage;
        return remainingHealth;
    }

    public int DealDamage(int damage)
    {
        return damage;
    }

    public void Death()
    {
        // Kill off the Unit once it's health reaches 0
        if (unitHealth <= 0)
        {
            // Play death animation + particle system
        }
    }

    public int Heal(int healing)
    {
        int gainedHealth = unitHealth + healing;

        return gainedHealth;
    }

    #endregion

    #region Unit Location Controller

    public void SetSelectionObject(bool value) => selectionObject.SetActive(value);

    // Give the unit other functions
    //void InitSelection() {
    //    myCamera = Camera.main;
    //    myAgent = GetComponent<NavMeshAgent>();

    //    // Instantiate the list
    //    SelectUnits.Instance.unitList.Add(gameObject);
    //}

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

    //void SendUnitToLocation() {
    //    if (Input.GetMouseButton(1)) {
    //        RaycastHit hit;
    //        Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);

    //        // Check if the Ray cast hit the ground
    //        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer)) {
    //            marker.transform.position = hit.point;

    //            // Set the destination of the AI to the marker
    //            myAgent.SetDestination(hit.point);
    //            print("Placed Marker");
    //        }
    //    }
    //}

    //void OnDestroy() {
    //    SelectUnits.Instance.unitList.Remove(this.gameObject);
    //}

    //void CustomSelectionUnits() {
    //    if(Input.GetMouseButtonDown(0))
    //    {
    //        RaycastHit hit;
    //        Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);

    //        if(Physics.Raycast(ray, out hit,Mathf.Infinity, clickableUnit)) {
    //            if(Input.GetKey(KeyCode.LeftShift)) {
    //                //wanneer shift ingedrukt wordt.
    //                Debug.Log("Multiple Units Selected");
    //                SelectUnits.Instance.ShiftClickSelect(hit.collider.gameObject);
    //            }
    //            else {
    //                //wanneer 1 keer geklikt.
    //                Debug.Log("Unit Selected");
    //                SelectUnits.Instance.ClickSelect(hit.collider.gameObject);
    //            }
    //        }
    //        else {
    //            //wanneer we missen en niet op shift klikken
    //            if (!Input.GetKey(KeyCode.LeftShift)) {
    //                SelectUnits.Instance.DeSelectAll();
    //            }
    //        }
    //    }
    //}

    #endregion
}
