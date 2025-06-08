using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject asteroidPrefab;
    public float asteroidDestroyTime = 10f;

    // Enemy
    public GameObject enemyPrefab;
    public float enemyDestroyTime = 10f;

    public float minInstantiateValue;
    public float maxInstantiateValue;

    [Header("Particle Effects")]
    public GameObject explosion;
    public GameObject muzzleFlash;
    [SerializeField] public GameObject starPrefab;

    [Header("Star Settings")]
    [SerializeField] private float starSpawnChance;
    private int currentStarCount = 0;
    private PlayerController playerController;

    public void Awake()
    {
        instance = this;
        playerController = Object.FindAnyObjectByType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("GameManager: PlayerController not found in the scene!");
        }
    }

    void Start()
    {
        InvokeRepeating("InstantiateAsteroids", 1f, 1f);
        InvokeRepeating("InstantiateEnemy", 1f, 1f);
    }

    void InstantiateAsteroids()
    {
        Vector3 asteroidpos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 6f);
        GameObject asteroid = Instantiate(asteroidPrefab, asteroidpos, Quaternion.identity);
        Destroy(asteroid, asteroidDestroyTime);
    }

    public void AsteroidDestroyed(Vector3 position)
    {
        Debug.Log("Asteroid destroyed at position: " + position);

        if (playerController != null && currentStarCount < playerController.MaxStarCount)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue <= starSpawnChance && starPrefab != null)
            {
                GameObject star = Instantiate(starPrefab, position, Quaternion.identity);
                star.GetComponent<Collider2D>().isTrigger = true;
                currentStarCount++;
                Debug.Log("Star spawned at position: " + position + ", Current star count: " + currentStarCount);
            }
            else if (starPrefab == null)
            {
                Debug.LogError("starPrefab is NULL! Ensure it is assigned in Inspector.");
            }
        }
        else if (playerController == null)
        {
            Debug.LogError("GameManager: PlayerController is null, cannot access maxStarCount.");
        }
    }

    public void StarCollected()
    {
        currentStarCount = Mathf.Max(0, currentStarCount - 1);
        Debug.Log("Star collected, Current star count: " + currentStarCount);
    }

    void InstantiateEnemy()
    {
        Vector3 enemyPos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 6f);
        GameObject enemy = Instantiate(enemyPrefab, enemyPos, Quaternion.Euler(0f, 0f, 180f));
        Destroy(enemy, enemyDestroyTime);
    }
}