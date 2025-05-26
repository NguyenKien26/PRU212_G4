using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    public int score = 0;

    [Header("Laser")]
    public GameObject laser;
    public Transform laserSpawnPosition;
    public float destroyTime = 5f;
    public Transform muzzleSpawnPosition;
    private void Update()
    {
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
            SpawnLaser();
            SpawnMuzzleFlash();
        }
    }
    void SpawnLaser()
    {
        Vector3 spawnOffset = laserSpawnPosition.up * 0.5f;
        //GameObject gm = Instantiate(laser, laserSpawnPosition);
        GameObject gm = Instantiate(laser, laserSpawnPosition.position + spawnOffset, laserSpawnPosition.rotation);
        gm.transform.SetParent(null);
        Destroy(gm, destroyTime);
    }
    void SpawnMuzzleFlash()
    {
        GameObject muzzle = Instantiate(GameManager.instance.muzzleFlash, muzzleSpawnPosition);
        muzzle.transform.SetParent(null);
        Destroy(muzzle, destroyTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroids") || collision.gameObject.CompareTag("Enemy"))
        {
            GameObject gm = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
            Destroy(gm, 2f);
            Destroy(collision.gameObject); // Phá hủy enemy/asteroid
            Destroy(this.gameObject);      // Player chết
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            GameObject gm = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
            Destroy(gm, 2f);
            score += 10;
            Destroy(collision.gameObject); // Chỉ item biến mất
        }
    }
}
