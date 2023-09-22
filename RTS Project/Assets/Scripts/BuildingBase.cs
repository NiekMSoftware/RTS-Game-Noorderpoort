using UnityEngine;

public class BuildingBase : MonoBehaviour
{
    [SerializeField] protected float buildingHp = 50f;
    [SerializeField] protected int maxWorkers = 5;
    [SerializeField] protected int currentWorkers = 0;
    [SerializeField] protected int maxStorage = 20;
    protected ItemSlot currentStorage;
    [SerializeField] protected ItemData item;

    private void Awake()
    {
        ItemSlot slot = new();
        slot.SetAmount(0);
        slot.SetData(item);
        currentStorage = slot;
    }

    protected void ManageStorage()
    {
        if (currentStorage.GetAmount() < maxStorage)
        {
            //human go work
        }

        //Human human;
        //human = GetComponent<Human>();

        //human.healthUnit;
    }

    public void AddItemToStorage(ItemData itemData)
    {
        if (itemData == item)
        {
            if (currentStorage.GetAmount() < maxStorage)
            {
                currentStorage.IncreaseAmount(1);
            }
            else
            {
                Debug.LogError("Storage full");
            }
        }
    }

    public void RemoveItemFromStorage(ItemData itemData)
    {
        if (itemData == item)
        {
            if (currentStorage.GetAmount() > 0)
            {
                currentStorage.IncreaseAmount(-1);
            }
            else
            {
                Debug.LogError("Storage Empty");
            }
        }
    }

    protected void AddHumanToBuilding()
    {
        currentWorkers++;
    }

    protected void RemoveHumanFromBuilding()
    {
        currentWorkers--;
    }

    void Start()
    {

    }

    void Update()
    {

    }
}