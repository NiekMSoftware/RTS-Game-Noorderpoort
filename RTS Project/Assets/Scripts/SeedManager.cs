using UnityEngine;

public class SeedManager : MonoBehaviour
{
    [SerializeField] private string gameSeed;
    [SerializeField] private int currentSeed;

    private void Awake()
    {
        currentSeed = gameSeed.GetHashCode();
        Random.InitState(currentSeed);
    }
}