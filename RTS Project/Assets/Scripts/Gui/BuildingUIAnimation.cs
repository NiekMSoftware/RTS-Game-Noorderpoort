using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuildingUIAnimation : MonoBehaviour
{
    public RectTransform buildingTab;
    public float animationSpeed;
    
    public void Animate() {
        Sequence animation = DOTween.Sequence();

        Vector3 position = buildingTab.position;
        position = new Vector3(position.x + buildingTab.rect.size.x, position.y, position.z);
    }
}
