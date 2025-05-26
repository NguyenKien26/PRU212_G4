using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject asteroidPrefab;
    public GameObject enemyPrefab;
    public float minInstantiateValue;
    public float maxInstantiateValue;
    public float asteroidDestroyTime = 10f;
    public float enemyDestroyTime = 10f;

    [Header("Particle Effects")]
    public GameObject explosion;
    public GameObject muzzleFlash;

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

    void InstantiateEnemy()
    {
        Vector3 enenmypos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 8f);
        GameObject enemy = Instantiate(enemyPrefab, enenmypos, Quaternion.Euler(0f,0f,180f));
        Destroy(enemy, enemyDestroyTime);
    }
}
