using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggler : MonoBehaviour
{
    public GameObject gameObj;

    public void OnButtonClick() {
        gameObj.SetActive(!gameObj.activeSelf);
    }
}
