using UnityEngine;

public class SeedManager : MonoBehaviour
{
    [SerializeField] private bool useCustomSeed = true;
    [SerializeField] private string gameSeed;
    [SerializeField] private int currentSeed;

    private void Awake()
    {
        if (!useCustomSeed) return;

        currentSeed = gameSeed.GetHashCode();
        Random.InitState(currentSeed);
    }
}