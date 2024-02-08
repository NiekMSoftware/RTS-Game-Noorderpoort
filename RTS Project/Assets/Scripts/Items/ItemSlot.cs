using System;
using UnityEngine;
[Serializable]
public class ItemSlot
{
    public ItemData data;
    public int amount;
    public int maxAmount;

    public int GetAmount() => amount;
    public int GetMaxAmount() => maxAmount;

    public void IncreaseAmount(int value) => amount += value;

    public void SetAmount(int value) => amount = value;
    public void SetMaxAmount(int value) => maxAmount = value;


    public ItemData GetData() => data;

    public ItemData SetData(ItemData _data) => data = _data;
}