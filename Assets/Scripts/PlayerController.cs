using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    public static bool isGameOver;

    public GameObject gameOverScreen;
    public GameObject gamePauseScreen;

    public float speed = 10f;

    [Header("Audio")]
    public AudioClip gameOverClip;
    private AudioSource audioSource;


    [Header("Laser")]
    public GameObject laser;
    public Transform laserSpawnPosition;
    public float destroyTime = 5f;
    public Transform muzzleSpawnPosition;

    private bool isInvincible = false;
    private Vector3 respawnPosition;

    private SpriteRenderer sr;
    private Collider2D col;

    private void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        isGameOver = false;

        respawnPosition = transform.position;

        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        audioSource = GetComponent<AudioSource>();

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);
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
        float yPos = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(xPos, yPos, 0) * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    void PlayrShoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject gm = Instantiate(laser, laserSpawnPosition.position, Quaternion.identity);
            Destroy(gm, destroyTime);

            GameObject muzzle = Instantiate(GameManager.instance.muzzleFlash, muzzleSpawnPosition.position, Quaternion.identity);
            Destroy(muzzle, destroyTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGameOver || isInvincible)
            return;

        if (collision.gameObject.CompareTag("Asteroids"))
        {
            HeartManager.life--;

            if (HeartManager.life <= 0)
            {
                isGameOver = true;

                GameObject exp = Instantiate(GameManager.instance.explosion, transform.position, Quaternion.identity);
                Destroy(exp, 2f);

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

        GameObject explosion = Instantiate(GameManager.instance.explosion, transform.position, Quaternion.identity);
        Destroy(explosion, 2f);

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


}