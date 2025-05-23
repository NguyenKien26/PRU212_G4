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
}
