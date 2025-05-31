using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    public void DropLoot()
    {
        foreach (LootItem item in lootTable)
        {
            float rand = Random.Range(0f, 100f);
            if (rand <= item.dropChance && item.itemPrefab != null)
            {
                Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
                break; // Chỉ rơi 1 item
            }
        }
    }

}
