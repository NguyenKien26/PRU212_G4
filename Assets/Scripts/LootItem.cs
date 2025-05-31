using UnityEngine;

[System.Serializable]
public class LootItem 
{
    public GameObject itemPrefab;
    [Range(0f, 100f)] public float dropChance;
}
