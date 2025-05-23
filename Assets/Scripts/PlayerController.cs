using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;

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
        GameObject gm = Instantiate(laser, laserSpawnPosition);
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
        if(collision.gameObject.tag == "Asteroids")
        {
            GameObject gm = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
            Destroy(gm, 2f);
            Destroy(this.gameObject);
        }
    }
}
