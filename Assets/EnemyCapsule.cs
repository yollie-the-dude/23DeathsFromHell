using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class EnemyCapsule : MonoBehaviour
{
    public int enemyToSpawn;
    public float Multiplier;
    [SerializeField] GameObject[] enemies;
    [SerializeField] ShakePreset Spawn;

    private void Start()
    {
        StartCoroutine(SpawnEnemy());
    }
    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(3f);
        GameObject GO = Instantiate(enemies[enemyToSpawn], transform.position, Quaternion.identity);
        GO.GetComponent<EnemyHp>().MaxHealth = Mathf.RoundToInt(GO.GetComponent<EnemyHp>().MaxHealth * Multiplier);
        Shaker.ShakeAll(Spawn);
        Destroy(gameObject);
    }
}
