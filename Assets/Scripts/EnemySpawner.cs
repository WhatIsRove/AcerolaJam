using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyObj;
    public Vector2 spawnRange;

    public float spawnTime;
    float timer;

    public int baseEnemyCount;
    public float waveMultiplier;
    public int maxWaves;
    int currentWave;
    int maxEnemiesForWave;
    public int currentEnemies;

    public bool allowSpawn;

    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemyText;

    void Start()
    {
        currentWave = 1;
        maxEnemiesForWave = baseEnemyCount + currentWave * (int)waveMultiplier;
        currentEnemies = maxEnemiesForWave;
        timer = spawnTime;

        waveText.text = "Wave " + GetRoman(currentWave) + " of " + GetRoman(maxWaves);
        enemyText.text = "Enemies Remaining: " + GetRoman(currentEnemies);

    }

    void Update()
    {
        if (allowSpawn && maxEnemiesForWave > 0 && currentWave <= maxWaves)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                Spawn();
                timer = spawnTime;
            }
        }
    }

    string GetRoman(int i)
    {
        if (i >= 40) return "XL" + GetRoman(i - 40);
        if (i >= 10) return "X" + GetRoman(i - 10);
        if (i >= 9) return "IX" + GetRoman(i - 9);
        if (i >= 5) return "V" + GetRoman(i - 5);
        if (i >= 4) return "IV" + GetRoman(i - 4);
        if (i >= 1) return "I" + GetRoman(i - 1);
        return string.Empty;
    }

    void Spawn()
    {
        Vector3 spawnPos = GetSpawnPos();
        spawnPos += FindObjectOfType<PlayerController>().transform.position;

        Instantiate(enemyObj, spawnPos, Quaternion.identity);
        maxEnemiesForWave--;
    }

    Vector3 GetSpawnPos()
    {
        var pos = new Vector3();

        int randomEdge = Random.Range(1, 3) % 2 == 0 ? -1 : 1;
        if (Random.Range(1, 3) % 2 == 0)
        {
            pos.x = Random.Range(-spawnRange.x, spawnRange.x);
            pos.z = spawnRange.y * randomEdge;
        } else
        {
            pos.z = Random.Range(-spawnRange.y, spawnRange.y);
            pos.x = spawnRange.x * randomEdge;
        }

        pos.y = 1;

        return pos;
    }

    public void ReduceEnemyCount()
    {
        currentEnemies--;
        enemyText.text = "Enemies Remaining: " + GetRoman(currentEnemies);

        if (currentEnemies == 0)
        {
            StartCoroutine(NextWave());
        }
    }

    IEnumerator NextWave()
    {
        yield return new WaitForSeconds(2f);
        currentWave++;
        if (currentWave > maxWaves)
        {
            FindObjectOfType<PlayerController>().StartNextLevel();
            yield break;
        }
        waveText.text = "Wave " + GetRoman(currentWave) + " of " + GetRoman(maxWaves);
        maxEnemiesForWave = baseEnemyCount + currentWave * (int)waveMultiplier;
        currentEnemies = maxEnemiesForWave;
        enemyText.text = "Enemies Remaining: " + GetRoman(currentEnemies);
    }
}
