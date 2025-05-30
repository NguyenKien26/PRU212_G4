using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject asteroidPrefab;
    public float minInstantiateValue;
    public float maxInstantiateValue;
    public float asteroidDestroyTime = 10f;

    [Header("Particle Effects")]
    public GameObject explosion;
    public GameObject muzzleFlash;
    [SerializeField] public GameObject starPrefab; // Prefab cho ngôi sao

    [Header("Star Settings")]
    [SerializeField] private float starSpawnChance = 1f;

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InvokeRepeating("InstantiateAsteroids", 1f, 1f);
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

        if (starPrefab != null)
        {
            GameObject star = Instantiate(starPrefab, position, Quaternion.identity);
            star.GetComponent<Collider2D>().isTrigger = true; // Đảm bảo có thể nhặt ngôi sao
            Debug.Log("Star spawned at position: " + position);
        }
        else
        {
            Debug.LogError("starPrefab is NULL! Ensure it is assigned in Inspector.");
        }

        //AddScore(5);
    }

}
