using System.Collections.Generic; // Cần thiết cho List
using UnityEngine;

// NUnit.Framework thường dùng cho unit testing trong Unity,
// nếu không có nhu cầu viết test, có thể xóa dòng này để code sạch hơn.
// using NUnit.Framework; 

public class EnemyController : MonoBehaviour
{
    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();

    [Header("Audio")]
    [SerializeField] private AudioClip destroySoundClip; // Âm thanh khi kẻ địch bị phá hủy

    // Thêm một Prefab riêng cho Star nếu bạn muốn luôn rơi ra Star
    // Bạn cần kéo Prefab Star vào đây trong Inspector
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
            // Hướng di chuyển: Enemy thường di chuyển XUỐNG.
            // Nếu bạn muốn chúng di chuyển xuống, hãy đổi Vector3.up thành Vector3.down. Nó up mới di chuyển xuống
            transform.Translate(Vector3.up * playerController.EnemySpeed * Time.deltaTime);
        }
        else
        {
            //Debug.LogWarning("EnemyController: PlayerController is null, cannot access enemySpeed. Enemy will not move.");
        }

        // Logic để hủy Enemy nếu nó đi ra khỏi màn hình (ví dụ: dưới đáy màn hình)
        // Điều chỉnh giá trị -10f tùy theo giới hạn màn hình của bạn
        if (transform.position.y < -10f)
        {
            Destroy(gameObject); // Hủy đối tượng khi nó ra khỏi màn hình
            Debug.Log($"{gameObject.name} went out of bounds and was destroyed.");
        }
    }

    // Phương thức xử lý va chạm của Enemy
    // Đảm bảo Collider2D của Enemy và Bullet đều là "Is Trigger"
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Untagged"))
        {
            // Phát âm thanh khi kẻ địch bị phá hủy
            if (destroySoundClip != null)
            {
                AudioSource.PlayClipAtPoint(destroySoundClip, transform.position, 1.0f);
            }
            else
            {
                Debug.LogWarning($"EnemyController: Destroy sound clip is not assigned for {gameObject.name}.");
            }

            // Gọi phương thức rơi đồ vật
            DropLoot();

            // Hủy đạn và kẻ địch
            Destroy(collision.gameObject); // Hủy đạn
            Destroy(gameObject);           // Hủy kẻ địch
            Debug.Log("Kẻ địch bị phá hủy bởi đạn.");
        }
    }


    public void DropLoot()
    {
        // Logic rơi Star riêng (nếu bạn muốn nó luôn rơi hoặc có xác suất cố định)
        if (starPrefab != null)
        {
            // Ví dụ: Luôn rơi Star, hoặc thêm Random.Range để có xác suất
            // if (Random.Range(0f, 100f) <= 70f) // 70% cơ hội rơi Star
            Instantiate(starPrefab, transform.position, Quaternion.identity);
            Debug.Log("Enemy dropped a Star.");
        }
        else
        {
            Debug.LogWarning("EnemyController: Star Prefab is not assigned, cannot drop Star.");
        }

        // Logic rơi các item khác từ Loot Table (nếu có)
        // Nếu bạn chỉ muốn rơi Star và không dùng LootTable, bạn có thể xóa vòng lặp này.
        foreach (LootItem item in lootTable)
        {
            float rand = UnityEngine.Random.Range(0f, 100f); // Sử dụng UnityEngine.Random
            if (rand <= item.dropChance && item.itemPrefab != null)
            {
                Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
                Debug.Log($"Enemy dropped {item.itemPrefab.name} from loot table.");
                break; // Thường chỉ muốn rơi một item mỗi lần
            }
        }
    }
}