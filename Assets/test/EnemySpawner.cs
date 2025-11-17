using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public MapLoader mapLoader;

    public void SpawnEnemy()
    {
        /* StageManager에서 ModifyDifficulty를 거친 EnemyPool을 받아와서 적절한 위치에 스폰 */
    }

    private void OnEnable()
    {
        mapLoader.OnMapLoaded += SpawnEnemy;
    }

    void Start()
    {

    }

    
    void Update()
    {
        
    }

    private void OnDisable()
    {
        mapLoader.OnMapLoaded -= SpawnEnemy;
    }
}
