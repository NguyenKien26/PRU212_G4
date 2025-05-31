using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static bool isGameOver;

    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gamePauseScreen;

    [SerializeField] private float speed = 10f;

    [Header("Audio")]
    [SerializeField] private AudioClip gameOverClip;
    private AudioSource audioSource;

    [Header("Laser")]
    [SerializeField] private GameObject laser;
    [SerializeField] private Transform laserSpawnPosition;
    [SerializeField] private float destroyTime = 5f;
    [SerializeField] private Transform muzzleSpawnPosition;

    private bool isInvincible = false;
    private Vector3 respawnPosition;

    private SpriteRenderer sr;
    private Collider2D col;

    private void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
        else
        {
            Debug.LogError("PlayerController: Thiếu Rigidbody2D!");
        }

        isGameOver = false;
        respawnPosition = transform.position;

        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        if (sr == null) Debug.LogError("PlayerController: Thiếu SpriteRenderer!");
        if (col == null) Debug.LogError("PlayerController: Thiếu Collider2D!");
        //if (audioSource == null) Debug.LogError("PlayerController: Thiếu AudioSource!");

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);
        else
            Debug.LogWarning("Game Over Screen chưa được gán!");
    }

    private void Update()
    {
        if (isGameOver)
            return;

        PlayrMovement();
        PlayrShoot();
    }

    void PlayrMovement()
    {
        float xPos = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(xPos, 0, 0) * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    void PlayrShoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (laser != null && laserSpawnPosition != null)
            {
                GameObject gm = Instantiate(laser, laserSpawnPosition.position, Quaternion.identity);
                Destroy(gm, destroyTime);
                Debug.Log($"Đã bắn laser tại {laserSpawnPosition.position}.");
            }
            else
            {
                Debug.LogError("Laser Prefab hoặc Laser Spawn Position chưa được gán!");
            }

            if (GameManager.instance != null && GameManager.instance.muzzleFlash != null && muzzleSpawnPosition != null)
            {
                GameObject muzzle = Instantiate(GameManager.instance.muzzleFlash, muzzleSpawnPosition.position, Quaternion.identity);
                Destroy(muzzle, destroyTime);
                Debug.Log("Đã tạo hiệu ứng muzzle flash.");
            }
            else
            {
                Debug.LogError("Muzzle Flash hoặc Muzzle Spawn Position chưa được gán!");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGameOver || isInvincible)
            return;

        if (collision.gameObject.CompareTag("Asteroids") || collision.gameObject.CompareTag("Enemy"))
        {
            HeartManager.life--;

            if (HeartManager.life <= 0)
            {
                isGameOver = true;

                if (GameManager.instance != null && GameManager.instance.explosion != null)
                {
                    GameObject exp = Instantiate(GameManager.instance.explosion, transform.position, Quaternion.identity);
                    Destroy(exp, 2f);
                }

                if (gameOverClip != null && audioSource != null)
                    audioSource.PlayOneShot(gameOverClip);

                if (gameOverScreen != null)
                    gameOverScreen.SetActive(true);

                gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(GetHurt());
            }
        }
    }

    IEnumerator GetHurt()
    {
        isInvincible = true;

        if (GameManager.instance != null && GameManager.instance.explosion != null)
        {
            GameObject explosion = Instantiate(GameManager.instance.explosion, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }

        sr.enabled = false;
        col.enabled = false;

        yield return new WaitForSeconds(1f);

        transform.position = respawnPosition;

        sr.enabled = true;
        col.enabled = false;

        Color originalColor = sr.color;
        int blinkCount = 5;
        float blinkTime = 0.2f;

        for (int i = 0; i < blinkCount; i++)
        {
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);
            yield return new WaitForSeconds(blinkTime);
            sr.color = originalColor;
            yield return new WaitForSeconds(blinkTime);
        }

        col.enabled = true;
        isInvincible = false;
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        if (gamePauseScreen != null)
            gamePauseScreen.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        if (gamePauseScreen != null)
            gamePauseScreen.SetActive(false);
    }

    public void MenuGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Star"))
        {
            Destroy(collision.gameObject); // Chỉ xóa star
            Debug.Log("Player collected a star!");
            // Nếu bạn muốn tăng điểm, thêm logic ở đây
        }
    }
}