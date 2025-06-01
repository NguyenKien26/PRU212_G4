using Assets.Scripts;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public TMP_Text scoreText;
    public Button restartButton;    
    public static bool isGameOver;
    private int score = 0;
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
            rb.freezeRotation = true;
        else
            Debug.LogError("PlayerController: Missing Rigidbody2D!");

        isGameOver = false;
        respawnPosition = transform.position;

        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        if (sr == null) Debug.LogError("PlayerController: Missing SpriteRenderer!");
        if (col == null) Debug.LogError("PlayerController: Missing Collider2D!");
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
    }

    private void Update()
    {
        if (isGameOver)
            return;

        PlayerMovement();
        PlayerShoot();
    }
    private void UpdateScoreUI()
    {
        scoreText.text = $"Score: {score}";
    }
    void PlayerMovement()
    {
        float xPos = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(xPos, 0, 0) * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    void PlayerShoot()
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

                SaveScoreToJSON();
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
        score = 0; // Reset điểm khi restart
        isGameOver = false;
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
            Destroy(collision.gameObject);
            score += 1;
            UpdateScoreUI();    
            Debug.Log($"Star collected! Current Score: {score}");
        }
    }
    void SaveScoreToJSON()
    {
        PlayerData playerData = new PlayerData { username = "DefaultUser", point = score, level = "Medium" };
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(Application.persistentDataPath + "/player_score.json", json);
        Debug.Log($"✅ Final Score saved to JSON: {score} points");
    }

}