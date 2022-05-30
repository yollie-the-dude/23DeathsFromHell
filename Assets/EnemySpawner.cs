using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
class Enemy
{
    public int ID;
    public float TimeThreshold;
    public bool localBool;
    public float SpawnCooldown;
}
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject Capsule;
    [SerializeField] Enemy[] enemyList;
    [SerializeField] float enemyHealthMultiplier;
    

    private void Update()
    {
        enemyHealthMultiplier = TimeManager.TimeLeft * 0.0035f + 1;
        for(int i = 0; i < enemyList.Length;i++)
        {
            if (TimeManager.TimeLeft > enemyList[i].TimeThreshold && !enemyList[i].localBool)
            {
                StartCoroutine(SpawnEnemy(i));
            }
        }

    }
    IEnumerator SpawnEnemy(int EnemyIndex)
    {
        enemyList[EnemyIndex].localBool = true;
        yield return new WaitForSeconds(enemyList[EnemyIndex].SpawnCooldown*0.5f);
        if(!TimeManager.pause)
        {
            GameObject go = Instantiate(Capsule, new Vector3(Random.Range(-50f, 50f), Random.Range(0.5f, 6f), Random.Range(-50f, 50f)), Quaternion.identity);
            go.GetComponent<EnemyCapsule>().enemyToSpawn = enemyList[EnemyIndex].ID;
            go.GetComponent<EnemyCapsule>().Multiplier = enemyHealthMultiplier;
            yield return new WaitForSeconds(enemyList[EnemyIndex].SpawnCooldown * 0.5f);
            enemyList[EnemyIndex].SpawnCooldown *= 0.9995f;
        }


        StartCoroutine(SpawnEnemy(EnemyIndex));

    }
    
}
