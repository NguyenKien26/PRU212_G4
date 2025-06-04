using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts;

public class PlayerController : MonoBehaviour
{
    public static bool isGameOver;

    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gamePauseScreen;
    [SerializeField] private float speed = 10.0f;

    [Header("Level Settings")]
    [SerializeField] private float asteroidSpeed = 5.0f; // Speed of asteroids for level customization
    [SerializeField] private float enemySpeed = 3.0f;    // Speed of enemies for level customization
    [SerializeField] private int maxStarCount = 10;      // Maximum number of collectible stars in the level

    public float AsteroidSpeed => asteroidSpeed;
    public float EnemySpeed => enemySpeed;
    public int MaxStarCount => maxStarCount;

    [Header("Audio")]
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip lazerClip;
    [SerializeField] private AudioClip newRecordClip;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip asteroidExplosionClip;
    [SerializeField] private AudioClip killEnermyClip;
    private AudioSource audioSource;

    [Header("Laser")]
    [SerializeField] private GameObject laser;
    [SerializeField] private Transform laserSpawnPosition;
    [SerializeField] private float destroyTime = 5.0f;
    [SerializeField] private Transform muzzleSpawnPosition;

    [SerializeField] private TMP_Text scoreTextUI;

    [SerializeField] private TMP_Text newRecordUI; 

    private bool isInvincible = false;
    private Vector3 respawnPosition;
    private SpriteRenderer sr;
    private Collider2D col;

    private int currentScore = 0;
    [SerializeField] private int level;
    private string username;

    private readonly string highScoreFileName = "high_score.json";

    private HighScoreData highScoreData;

    private void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.freezeRotation = true;
        else Debug.LogError("PlayerController: Rigidbody2D component is missing!");

        isGameOver = false;
        respawnPosition = transform.position;

        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        if (sr == null) Debug.LogError("PlayerController: SpriteRenderer component is missing!");
        if (col == null) Debug.LogError("PlayerController: Collider2D component is missing!");
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("PlayerController: AudioSource component is missing, automatically added.");
        }

        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        else Debug.LogWarning("Game Over Screen GameObject is not assigned!");

        if (gamePauseScreen != null) gamePauseScreen.SetActive(false);
        else Debug.LogWarning("Game Pause Screen GameObject is not assigned!");

        // Khởi tạo trạng thái của newRecordUI
        if (newRecordUI != null) newRecordUI.gameObject.SetActive(false); // Đảm bảo ban đầu nó ẩn
        else Debug.LogWarning("New Record UI Text (TMP_Text) is not assigned!");


        username = SystemInfo.deviceName;
        if (string.IsNullOrEmpty(username)) username = "You";

        currentScore = 0;
        UpdateScoreUI();
    }

    private void Update()
    {
        if (isGameOver) return;

        PlayrMovement();
        PlayrShoot();
    }

    private void PlayrMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(horizontalInput, 0, 0) * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    private void PlayrShoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (laser != null && laserSpawnPosition != null)
            {
                GameObject laserInstance = Instantiate(laser, laserSpawnPosition.position, Quaternion.identity);
                Destroy(laserInstance, destroyTime);
                Debug.Log($"Laser fired at {laserSpawnPosition.position}.");
                if (lazerClip != null && audioSource != null)
                    audioSource.PlayOneShot(lazerClip);
            }
            else Debug.LogError("Laser Prefab or Laser Spawn Position is not assigned!");

            if (GameManager.instance != null && GameManager.instance.muzzleFlash != null && muzzleSpawnPosition != null)
            {
                GameObject muzzleFlashInstance = Instantiate(GameManager.instance.muzzleFlash, muzzleSpawnPosition.position, Quaternion.identity);
                Destroy(muzzleFlashInstance, destroyTime);
                Debug.Log("Muzzle flash effect created.");
            }
            else Debug.LogError("Muzzle Flash Prefab or Muzzle Spawn Position is not assigned!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGameOver || isInvincible) return;

        if (collision.gameObject.CompareTag("Asteroids") || collision.gameObject.CompareTag("Enemy"))
        {
            // Phát tiếng nổ thiên thạch/kẻ địch khi va chạm
            if (asteroidExplosionClip != null)
            {
                // Sử dụng PlayClipAtPoint để phát âm thanh nổ tại vị trí va chạm
                AudioSource.PlayClipAtPoint(asteroidExplosionClip, collision.transform.position);
            }

            HeartManager.life--;

            if (HeartManager.life <= 0)
            {
                isGameOver = true;
                SaveHighScore(); // Check and save high score on game over

                if (GameManager.instance != null && GameManager.instance.explosion != null)
                {
                    GameObject explosionInstance = Instantiate(GameManager.instance.explosion, transform.position, Quaternion.identity);
                    Destroy(explosionInstance, 2f);
                }

                if (gameOverClip != null && audioSource != null)
                    audioSource.PlayOneShot(gameOverClip);

                if (gameOverScreen != null)
                    gameOverScreen.SetActive(true);

                gameObject.SetActive(false);
            }
            else
            {
                // Phát tiếng người chơi bị thương (hurt)
                if (hurtClip != null && audioSource != null)
                    audioSource.PlayOneShot(hurtClip);

                StartCoroutine(GetHurt());
            }
        }
    }

    private IEnumerator GetHurt()
    {
        isInvincible = true;

        if (GameManager.instance != null && GameManager.instance.explosion != null)
        {
            GameObject explosionInstance = Instantiate(GameManager.instance.explosion, transform.position, Quaternion.identity);
            Destroy(explosionInstance, 2f);
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
        // Đảm bảo newRecordUI ẩn khi chơi lại
        if (newRecordUI != null) newRecordUI.gameObject.SetActive(false);
    }
    public void PauseGame() { Time.timeScale = 0; if (gamePauseScreen != null) gamePauseScreen.SetActive(true); }
    public void ResumeGame() { Time.timeScale = 1; if (gamePauseScreen != null) gamePauseScreen.SetActive(false); }
    public void MenuGame()
    {
        SceneManager.LoadScene("MainMenu");
        // Đảm bảo newRecordUI ẩn khi về menu
        if (newRecordUI != null) newRecordUI.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Star"))
        {
            Destroy(collision.gameObject);
            Debug.Log("Star collected!");
            currentScore++;
            UpdateScoreUI();
            if (GameManager.instance != null)
            {
                GameManager.instance.StarCollected();
            }
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreTextUI != null) scoreTextUI.text = "Score: " + currentScore.ToString();
        else Debug.LogError("Score Text UI component is not assigned!");
    }

    private void LoadHighScore()
    {
        string filePath = Path.Combine(Application.persistentDataPath, highScoreFileName);
        if (File.Exists(filePath))
        {
            try
            {
                string jsonData = File.ReadAllText(filePath);
                highScoreData = JsonUtility.FromJson<HighScoreData>(jsonData);
                Debug.Log("High score loaded from " + filePath + $": Username={highScoreData.Username}, Level={highScoreData.Level}, Score={highScoreData.Score}");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load high score from " + filePath + ": " + e.Message);
            }
        }
        else
        {
            highScoreData = new HighScoreData { Username = "N/A", Level = 1, Score = 0 };
            SaveHighScore(); // Create default high score file
            Debug.Log("No high score file found at " + filePath + ", created default.");
        }
    }

    private void SaveHighScore()
    {
        // Kiểm tra xem có lập kỷ lục mới không
        bool isNewRecord = (highScoreData == null || currentScore > highScoreData.Score);

        if (isNewRecord)
        {
            highScoreData = new HighScoreData { Username = "You", Level = level, Score = currentScore };
            string filePath = Path.Combine(Application.persistentDataPath, highScoreFileName);
            string jsonData = JsonUtility.ToJson(highScoreData);
            try
            {
                File.WriteAllText(filePath, jsonData);
                Debug.Log("New high score saved to " + filePath + $" - Username: {username}, Level: {level}, Score: {currentScore}");

                // Phát âm thanh New Record
                if (newRecordClip != null && audioSource != null)
                    audioSource.PlayOneShot(newRecordClip);

                // Hiển thị UI New Record
                if (newRecordUI != null)
                {
                    newRecordUI.text = "NEW RECORD: " + currentScore.ToString();
                    newRecordUI.gameObject.SetActive(true);
                    StartCoroutine(HideNewRecordUI(3f)); // Ẩn sau 3 giây
                }
                else Debug.LogWarning("New Record UI Text (TMP_Text) is not assigned, cannot display new record message!");

            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save high score to " + filePath + ": " + e.Message);
            }
        }
        else Debug.Log($"Current score ({currentScore}) is not higher than the high score ({highScoreData.Score}).");
    }

    // Coroutine để ẩn newRecordUI sau một thời gian
    private IEnumerator HideNewRecordUI(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (newRecordUI != null)
        {
            newRecordUI.gameObject.SetActive(false);
        }
    }
}