using System.Collections.Generic; 
using UnityEngine;



public class EnemyController : MonoBehaviour
{
    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();

    [Header("Audio")]
    [SerializeField] private AudioClip destroySoundClip; 



    [SerializeField] private GameObject starPrefab;

    private PlayerController playerController;

    private void Start()
    {
        playerController = Object.FindAnyObjectByType<PlayerController>();
        if (playerController == null)
        {
            //Debug.LogError("EnemyController: PlayerController not found in the scene!");
        }
    }

    void Update()
    {
        if (playerController != null)
        {
            transform.Translate(Vector3.up * playerController.EnemySpeed * Time.deltaTime);
        }
        else
        {
            
        }
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
            Debug.Log($"{gameObject.name} went out of bounds and was destroyed.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Untagged"))
        {
            if (destroySoundClip != null)
            {
                AudioSource.PlayClipAtPoint(destroySoundClip, transform.position, 1.0f);
            }
            else
            {
                Debug.LogWarning($"EnemyController: Destroy sound clip is not assigned for {gameObject.name}.");
            }

            DropLoot();

            Destroy(collision.gameObject);
            Destroy(gameObject);          
            Debug.Log("Kẻ địch bị phá hủy bởi đạn.");
        }
    }


    public void DropLoot()
    {
        if (starPrefab != null)
        {
            Instantiate(starPrefab, transform.position, Quaternion.identity);
            Debug.Log("Enemy dropped a Star.");
        }
        else
        {
            Debug.LogWarning("EnemyController: Star Prefab is not assigned, cannot drop Star.");
        }

        foreach (LootItem item in lootTable)
        {
            float rand = UnityEngine.Random.Range(0f, 100f); // Sử dụng UnityEngine.Random
            if (rand <= item.dropChance && item.itemPrefab != null)
            {
                Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
                Debug.Log($"Enemy dropped {item.itemPrefab.name} from loot table.");
                break; 
            }
        }
    }
}