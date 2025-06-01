using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //Asteroid
    public GameObject asteroidPrefab;
    public float asteroidDestroyTime = 10f;


    //Enemy
    public GameObject enemyPrefab;
    public float enemyDestroyTime = 10f;

    public float minInstantiateValue;
    public float maxInstantiateValue;

    [Header("Particle Effects")]
    public GameObject explosion;
    public GameObject muzzleFlash;
    [SerializeField] public GameObject starPrefab; // Prefab cho ngôi sao

    [Header("Star Settings")]
    [SerializeField] private float starSpawnChance;

    public void Awake()
    {
        instance = this;
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

        //float randomValue = Random.Range(0f, 1f);
        //if (randomValue <= 0.3f)
        //{
            if (starPrefab != null)
            {
                GameObject star = Instantiate(starPrefab, position, Quaternion.identity);
                star.GetComponent<Collider2D>().isTrigger = true;
                //Destroy(star, 5f);
                Debug.Log("Star spawned at position: " + position);
            }
            else
            {
                Debug.LogError("starPrefab is NULL! Ensure it is assigned in Inspector.");
            }
        //}
        
    }
        //AddScore(5);

    void InstantiateEnemy()
    {
        Vector3 enemyPos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 6f);
        GameObject enemy = Instantiate(enemyPrefab, enemyPos, Quaternion.Euler(0f, 0f, 180f));
        Destroy(enemy, enemyDestroyTime);
    }
}
