using UnityEngine;

public class TestAISpawner : MonoBehaviour
{
    [SerializeField] private GameObject aiToSpawn;
    [SerializeField] private GameObject SoldierToSpawn;
    [SerializeField] private LayerMask ground;
    [SerializeField] private Terrain terrain;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
            {
                Vector3 spawnPos = hit.point;
                spawnPos.y = terrain.SampleHeight(hit.point) + aiToSpawn.transform.localScale.y;
                GameObject spawnedAI = Instantiate(aiToSpawn, spawnPos, Quaternion.identity);
                spawnedAI.GetComponent<Unit>().typeUnit = Unit.TypeUnit.Human;
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
            {
                Vector3 spawnPos = hit.point;
                spawnPos.y = terrain.SampleHeight(hit.point) + SoldierToSpawn.transform.localScale.y;
                GameObject spawnedAI = Instantiate(SoldierToSpawn, spawnPos, Quaternion.identity);
                spawnedAI.GetComponent<Unit>().typeUnit = Unit.TypeUnit.Human;
            }
        }
    }
}