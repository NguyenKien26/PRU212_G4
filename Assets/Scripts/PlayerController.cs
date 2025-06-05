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
    [SerializeField] private AudioClip starCollectClip;
    [SerializeField] private AudioClip engineClip;

    private AudioSource engineAudioSource;
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
        else Debug.LogError("PlayerController: Rigidbody2D missing!");

        isGameOver = false;
        respawnPosition = transform.position;

        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        if (sr == null) Debug.LogError("SpriteRenderer missing!");
        if (col == null) Debug.LogError("Collider2D missing!");
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            //Debug.LogWarning("AudioSource auto-added.");
        }

        // Setup engine audio
        engineAudioSource = gameObject.AddComponent<AudioSource>();
        engineAudioSource.clip = engineClip;
        engineAudioSource.loop = true;
        engineAudioSource.playOnAwake = false;
        if (engineClip != null) engineAudioSource.Play(); engineAudioSource.Pause();

        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (gamePauseScreen != null) gamePauseScreen.SetActive(false);
        if (newRecordUI != null) newRecordUI.gameObject.SetActive(false);

        username = SystemInfo.deviceName;
        if (string.IsNullOrEmpty(username)) username = "You";

        currentScore = 0;
        UpdateScoreUI();

        // Set level based on scene
        level = SceneManager.GetActiveScene().buildIndex;
        LoadHighScore();
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
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0) * speed * Time.deltaTime;
        transform.Translate(movement);

        // Engine sound control
        if (engineAudioSource != null)
        {
            if (movement.magnitude > 0.01f)
            {
                if (!engineAudioSource.isPlaying) engineAudioSource.UnPause();
            }
            else
            {
                if (engineAudioSource.isPlaying) engineAudioSource.Pause();
            }
        }
    }


    private void PlayrShoot()
    {
        if (!sr.enabled) return; // Nếu nhân vật chưa hồi sinh xong không cho bắn 

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (laser != null && laserSpawnPosition != null)
            {
                GameObject laserInstance = Instantiate(laser, laserSpawnPosition.position, Quaternion.identity);
                Destroy(laserInstance, destroyTime);
                if (lazerClip != null && audioSource != null)
                    audioSource.PlayOneShot(lazerClip);
            }
            else Debug.LogError("Laser Prefab or Laser Spawn Position is not assigned!");

            if (GameManager.instance != null && GameManager.instance.muzzleFlash != null && muzzleSpawnPosition != null)
            {
                GameObject muzzleFlashInstance = Instantiate(GameManager.instance.muzzleFlash, muzzleSpawnPosition.position, Quaternion.identity);
                Destroy(muzzleFlashInstance, destroyTime);
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
        // Dừng tất cả coroutine
        StopAllCoroutines();

        // Đặt lại Time.timeScale
        Time.timeScale = 1;

        // Ẩn newRecordUI nếu tồn tại
        if (newRecordUI != null)
        {
            newRecordUI.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("newRecordUI is not assigned in Inspector!");
        }

        Debug.Log($"Reloading scene: {SceneManager.GetActiveScene().buildIndex}");

        try
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to reload scene: {e.Message}");
        }
    }

    // Sửa coroutine HideNewRecordUI để sử dụng WaitForSecondsRealtime
    private IEnumerator HideNewRecordUI(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (newRecordUI != null)
        {
            newRecordUI.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("newRecordUI is not assigned when hiding!");
        }
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        if (gamePauseScreen != null)
        {
            gamePauseScreen.SetActive(true);
        }
        else
        {
            Debug.LogError("gamePauseScreen is not assigned in Inspector!");
        }
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        if (gamePauseScreen != null)
        {
            gamePauseScreen.SetActive(false);
        }
        else
        {
            Debug.LogError("gamePauseScreen is not assigned in Inspector!");
        }
    }
    public void MenuGame()
    {
        // Dừng tất cả coroutine để tránh truy cập đối tượng đã hủy
        StopAllCoroutines();

        // Đặt lại Time.timeScale trước khi chuyển cảnh
        Time.timeScale = 1;

        // Ẩn newRecordUI nếu tồn tại
        if (newRecordUI != null)
        {
            newRecordUI.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("newRecordUI is not assigned in Inspector!");
        }

        // Ghi log để debug
        Debug.Log("Loading MainMenu scene");

        try
        {
            SceneManager.LoadScene("MainMenu");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load MainMenu scene: {e.Message}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Star"))
        {
            Destroy(collision.gameObject);
            Debug.Log("Star collected!");
            currentScore++;
            UpdateScoreUI();

            // Phát âm thanh nhặt sao
            if (starCollectClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(starCollectClip);
            }
            else
            {
                Debug.LogWarning("Star collect clip hoặc AudioSource chưa được gán!");
            }

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
        highScoreData = new HighScoreData(); // Initialize with empty Scores list

        if (File.Exists(filePath))
        {
            try
            {
                string jsonData = File.ReadAllText(filePath);
                HighScoreData loadedData = JsonUtility.FromJson<HighScoreData>(jsonData);
                if (loadedData != null && loadedData.Scores != null)
                {
                    highScoreData = loadedData;
                    Debug.Log($"High score loaded from {filePath}: {highScoreData.Scores.Count} levels");
                }
                else
                {
                    Debug.LogWarning("Loaded high score data is invalid, initializing defaults.");
                    InitializeDefaultHighScore();
                    SaveHighScore();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load high score from {filePath}: {e.Message}");
                InitializeDefaultHighScore();
                SaveHighScore();
            }
        }
        else
        {
            Debug.Log($"No high score file found at {filePath}, creating default.");
            InitializeDefaultHighScore();
            SaveHighScore();
        }
    }
    private void InitializeDefaultHighScore()
    {
        highScoreData = new HighScoreData();
        highScoreData.Scores.Add(new HighScoreData.LevelScore { Username = "N/A", Level = 1, Score = 0 });
        highScoreData.Scores.Add(new HighScoreData.LevelScore { Username = "N/A", Level = 2, Score = 0 });
    }

    private void SaveHighScore()
    {
        if (highScoreData == null)
        {
            Debug.LogWarning("highScoreData is null, initializing default.");
            InitializeDefaultHighScore();
        }

        var levelScore = highScoreData.Scores.Find(s => s.Level == level);
        bool isNewRecord = (levelScore == null || currentScore > levelScore.Score);

        if (isNewRecord)
        {
            if (levelScore == null)
            {
                levelScore = new HighScoreData.LevelScore { Username = username, Level = level, Score = currentScore };
                highScoreData.Scores.Add(levelScore);
            }
            else
            {
                levelScore.Username = username;
                levelScore.Score = currentScore;
            }

            string filePath = Path.Combine(Application.persistentDataPath, highScoreFileName);
            string jsonData = JsonUtility.ToJson(highScoreData, true);
            try
            {
                File.WriteAllText(filePath, jsonData);
                Debug.Log($"New high score saved for Level {level}: {currentScore}");

                if (newRecordClip != null && audioSource != null)
                    audioSource.PlayOneShot(newRecordClip);

                if (newRecordUI != null)
                {
                    newRecordUI.text = $"NEW RECORD (Level {level}): {currentScore}";
                    newRecordUI.gameObject.SetActive(true);
                    //StartCoroutine(HideNewRecordUI(3f));
                }
                else
                {
                    Debug.LogWarning("New Record UI Text (TMP_Text) is not assigned, cannot display new record message!");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save high score to {filePath}: {e.Message}");
            }
        }
        else
        {
            var currentHighScore = levelScore != null ? levelScore.Score : 0;
            Debug.Log($"Current score ({currentScore}) is not higher than the high score ({currentHighScore}) for Level {level}.");
            if (newRecordUI != null)
            {
                newRecordUI.text = $"HIGHEST SCORE (Level {level}): {currentHighScore}";
                newRecordUI.gameObject.SetActive(true);
                StartCoroutine(HideNewRecordUI(3f));
            }
            else
            {
                Debug.LogWarning("New Record UI Text (TMP_Text) is not assigned, cannot display highest score message!");
            }
        }
    }
}