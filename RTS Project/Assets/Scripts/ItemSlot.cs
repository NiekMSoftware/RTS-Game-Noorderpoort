using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private ItemData data;
    [SerializeField] private int amount;

    public int GetAmount() => amount;

    public void IncreaseAmount(int value) => amount += value;

    public ItemData GetData() => data;
}