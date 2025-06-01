using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class PlayerScoreData
{
    public string Username;
    public int Level;
    public int Score;
}

[Serializable]
public class HighScoreData
{
    public string Username;
    public int Level;
    public int Score;
}

public class PlayerController : MonoBehaviour
{
    public static bool isGameOver;

    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gamePauseScreen;
    [SerializeField] private float speed = 10.0f;

    [Header("Audio")]
    [SerializeField] private AudioClip gameOverClip;
    private AudioSource audioSource;

    [Header("Laser")]
    [SerializeField] private GameObject laser;
    [SerializeField] private Transform laserSpawnPosition;
    [SerializeField] private float destroyTime = 5.0f;
    [SerializeField] private Transform muzzleSpawnPosition;


     [SerializeField] private TMP_Text scoreTextUI;

    private bool isInvincible = false;
    private Vector3 respawnPosition;
    private SpriteRenderer sr;
    private Collider2D col;

    private int currentScore = 0;
    private int level = 1;
    private string username;

    private readonly string playerScoreFileName = "player_score.json";
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

        username = SystemInfo.deviceName;
        if (string.IsNullOrEmpty(username)) username = "You";

        LoadPlayerScore();
        LoadHighScore();
        UpdateScoreUI();
    }

    private void Update()
    {
        if (isGameOver) return;

        PlayrMovement();
        PlayrShoot();

        // Save current player data (score, level, username)
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerScore();
            Debug.Log("Player data saved locally.");
        }

        // Load saved player data
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerScore();
            UpdateScoreUI();
            Debug.Log("Player data loaded locally.");
        }
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
            else StartCoroutine(GetHurt());
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

    public void ReplayGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public void PauseGame() { Time.timeScale = 0; if (gamePauseScreen != null) gamePauseScreen.SetActive(true); }
    public void ResumeGame() { Time.timeScale = 1; if (gamePauseScreen != null) gamePauseScreen.SetActive(false); }
    public void MenuGame() => SceneManager.LoadScene("MainMenu");

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Star"))
        {
            Destroy(collision.gameObject);
            Debug.Log("Star collected!");
            currentScore++;
            UpdateScoreUI();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreTextUI != null) scoreTextUI.text = "Score: " + currentScore.ToString();
        else Debug.LogError("Score Text UI component is not assigned!");
    }

    private PlayerScoreData GetPlayerScoreData()
    {
        return new PlayerScoreData { Username = username, Level = level, Score = currentScore };
    }

    private void SavePlayerScore()
    {
        PlayerScoreData dataToSave = GetPlayerScoreData();
        string filePath = Path.Combine(Application.persistentDataPath, playerScoreFileName);
        string jsonData = JsonUtility.ToJson(dataToSave);
        try
        {
            File.WriteAllText(filePath, jsonData);
            Debug.Log("Player data saved to " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save player data to " + filePath + ": " + e.Message);
        }
    }

    private void LoadPlayerScore()
    {
        string filePath = Path.Combine(Application.persistentDataPath, playerScoreFileName);
        if (File.Exists(filePath))
        {
            try
            {
                string jsonData = File.ReadAllText(filePath);
                PlayerScoreData loadedData = JsonUtility.FromJson<PlayerScoreData>(jsonData);
                if (loadedData != null)
                {
                    username = loadedData.Username;
                    level = loadedData.Level;
                    currentScore = loadedData.Score;
                    Debug.Log("Player data loaded from " + filePath + $": Username={username}, Level={level}, Score={currentScore}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load player data from " + filePath + ": " + e.Message);
            }
        }
        else Debug.Log("No player data file found at " + filePath);
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
        if (highScoreData == null || currentScore > highScoreData.Score)
        {
            highScoreData = new HighScoreData { Username = username, Level = level, Score = currentScore };
            string filePath = Path.Combine(Application.persistentDataPath, highScoreFileName);
            string jsonData = JsonUtility.ToJson(highScoreData);
            try
            {
                File.WriteAllText(filePath, jsonData);
                Debug.Log("New high score saved to " + filePath + $" - Username: {username}, Level: {level}, Score: {currentScore}");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save high score to " + filePath + ": " + e.Message);
            }
        }
        else Debug.Log($"Current score ({currentScore}) is not higher than the high score ({highScoreData.Score}).");
    }
}