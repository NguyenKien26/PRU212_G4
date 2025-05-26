using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    
    public float enemySpeed;

    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * enemySpeed * Time.deltaTime);
    }

    public void OnHitByLaser()
    {
        TryDropLoot();
        Destroy(gameObject); // Xoá Enemy
    }

    void TryDropLoot()
    {
        foreach (var loot in lootTable)
        {
            float roll = Random.Range(0f, 100f);
            if (roll <= loot.dropChance)
            {
                Instantiate(loot.itemPrefab, transform.position, Quaternion.identity);
                break;
            }
        }
    }

}
