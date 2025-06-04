using UnityEngine;

public class AsteroidsController : MonoBehaviour
{
    private PlayerController playerController;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip explosionClip;

    [Header("Asteroid Sprites")]
    [SerializeField] private Sprite[] asteroidSprites;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 100f;

    private SpriteRenderer spriteRenderer;
    private bool isDestroyed = false;

    private void Start()
    {
        playerController = Object.FindAnyObjectByType<PlayerController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // lấy sprite con (hoặc GetComponent nếu trên cùng object)

        if (playerController == null)
        {
            Debug.LogError("AsteroidsController: PlayerController không tìm thấy trong scene!");
        }

        if (spriteRenderer == null)
        {
            Debug.LogError("AsteroidsController: Không tìm thấy SpriteRenderer!");
        }

        if (asteroidSprites != null && asteroidSprites.Length > 0)
        {
            int index = Random.Range(0, asteroidSprites.Length);
            spriteRenderer.sprite = asteroidSprites[index];
        }
        else
        {
            Debug.LogWarning("AsteroidsController: asteroidSprites chưa được gán hoặc rỗng!");
        }
    }

    private void Update()
    {
        if (isDestroyed) return;

        // Xoay sprite riêng (không xoay cả object)
        if (spriteRenderer != null)
        {
            spriteRenderer.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }

        // Di chuyển thiên thạch thẳng xuống (toàn bộ object)
        if (playerController != null)
        {
            transform.Translate(Vector3.down * playerController.AsteroidSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Untagged"))
        {
            if (GameManager.instance != null)
            {
                isDestroyed = true;

                GameManager.instance.AsteroidDestroyed(transform.position);

                if (explosionClip != null)
                {
                    AudioSource.PlayClipAtPoint(explosionClip, transform.position);
                }
                else
                {
                    Debug.LogWarning("AsteroidsController: explosionClip chưa được gán!");
                }

                Destroy(collision.gameObject);
                Destroy(gameObject, 0.5f);

                Debug.Log("Thiên thạch bị phá hủy bởi đạn.");
            }
            else
            {
                Debug.LogError("GameManager instance là null.");
            }
        }
    }
}
