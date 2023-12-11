using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
[System.Serializable]
public class ItemData : ScriptableObject
{
    [SerializeField] private string itemName;
}