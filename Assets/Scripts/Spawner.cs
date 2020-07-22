using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves;
    public Enemy enemy;
    Wave currentWave;
    int currentWaveNumber;

    int enemyRemainingToSpawn;
    int enemyRemainingAlive;
    float nextSpawnTime;
    private void Start()
    {
        NextWave();
    }

    private void Update()
    {
        if (enemyRemainingToSpawn>0&&Time.time>nextSpawnTime)
        {
            enemyRemainingToSpawn--;
            nextSpawnTime = Time.time+ currentWave.timeBetweenSpawns;

            Enemy spawnEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
            spawnEnemy.OnDeath += OnEnemyDeath;
        }
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
        }
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCuont;
        public float timeBetweenSpawns;
    }
}
