using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool devMode;

    public Wave[] waves;
    public Enemy enemy;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave;
    int currentWaveNumber;

    int enemyRemainingToSpawn;
    int enemyRemainingAlive;
    float nextSpawnTime;

    MapGenerator map;

    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositinOld;
    bool isCamp;

    bool isdisabled;

    public event System.Action<int> OnNewWave;

    private void Start()
    {
       
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositinOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    private void Update()
    {
        if (!isdisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamp = (Vector3.Distance(campPositinOld, playerT.position) < campThresholdDistance);
                campPositinOld = playerT.position;
            }

            if ((enemyRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemyRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine("SpawnEnemy");
            }
        }

        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform spawnTile = map.GetRandomOpenTile();
        if (isCamp)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColour = Color.white;
        Color flashColour = Color.red;
        float spawnTimer = 0;

        while (spawnTimer<spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColour,flashColour,Mathf.PingPong(spawnTimer*tileFlashSpeed,1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnEnemy = Instantiate(enemy, spawnTile.position+Vector3.up, Quaternion.identity) as Enemy;
        spawnEnemy.OnDeath += OnEnemyDeath;
        spawnEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsTokillPlayer, currentWave.enemyHeath, currentWave.skinColour);
    }

    public void OnPlayerDeath()
    {
        isdisabled = true;
    }

    public void OnEnemyDeath()
    {
        enemyRemainingAlive--;
        if (enemyRemainingAlive==0)
        {
            NextWave();
        }
    }

    public void NextWave()
    {
        currentWaveNumber++;
        if (currentWaveNumber-1<waves.Length)
        {
            //Debug.Log("Wave:" + currentWaveNumber);
            currentWave = waves[currentWaveNumber - 1];

            enemyRemainingToSpawn = currentWave.enemyCuont;
            enemyRemainingAlive = enemyRemainingToSpawn;

            if (OnNewWave!=null)
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }
    }

    void ResetPlayerPosition()
    {
        playerT.position = map.CentrePosition()+Vector3.up*2;
    }

    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCuont;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsTokillPlayer;
        public float enemyHeath;
        public Color skinColour;
    }
}
