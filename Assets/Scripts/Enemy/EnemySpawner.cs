using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups; //List of groups of enemies to spawn in this wave
        public int waveQuota; //Number of enemies
        public float spawnInterval; //Spawn interval
        public int spawnCount; //Number of already spawned Enemies

    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount;
        public int spawnCount;
        public GameObject enemyPrefab;
    }

    public List<Wave> waves; //List of all waves in game
    public int currentWaveCount; //Current wave index

    [Header("spawner Attributes")]
    float SpawnTimer; //Timer used to spawn the next enemy
    public int enemiesAlive;
    public int maxEnemiesAllowed;
    public bool maxEnemiesReached = false;
    public float waveInterval; //between each wave

    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPoints;

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        CalculateWaveQuota();
        firstWave();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0)
        {
            StartCoroutine(BeginNextWave());
        }

        SpawnTimer += Time.deltaTime;
        
        //Check if enemy can be spawned
        if(SpawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            SpawnTimer = 0f;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        yield return new WaitForSeconds(waveInterval);

        if(currentWaveCount < waves.Count - 1)
        {
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach(var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;
        Debug.LogWarning(currentWaveQuota);
    }


    void firstWave()
    {
        if(currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0)
        {
            StartCoroutine(BeginNextWave());
        }

        SpawnTimer = 0f;
        SpawnEnemies();
    }


    void SpawnEnemies()
    {
        //Check if if minimum number of enemies have been spawned
        if(waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            //Spawn each type of enemy till quota is hit
            foreach(var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                //check if min num of enemies of this type have been spawned
                if(enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    if(enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }

                    int randomIndex = Random.Range(0, relativeSpawnPoints.Count);
                    UnityEngine.Vector3 spawnOffset = relativeSpawnPoints[randomIndex].localPosition; // Use localPosition for relative offset
                    UnityEngine.Vector3 spawnPosition = player.position + spawnOffset; // Add offset to player's position

                    Instantiate(enemyGroup.enemyPrefab, spawnPosition, UnityEngine.Quaternion.identity);
                    //Spawn enemies at random positions close to the player
                    //Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(1, relativeSpawnPoints.Count)].position, UnityEngine.Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;
                }
            }
        }

        if(enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
    }
}
