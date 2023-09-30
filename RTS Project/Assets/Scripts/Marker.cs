using System.Collections;
using UnityEngine;

public class Marker : MonoBehaviour
{
    [SerializeField] private float destroyTime;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(destroyTime);

        Destroy(gameObject);

        yield return null;
    }
}