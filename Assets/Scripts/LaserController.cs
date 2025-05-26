using UnityEngine;

public class LaserController : MonoBehaviour
{
    public float laserSpeed = 25f;
    void Update()
    {
        transform.Translate(Vector3.up * laserSpeed * Time.deltaTime);   
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.OnHitByLaser(); // Gọi xử lý rơi vật phẩm
            }
        }
        else if (collision.gameObject.tag == "Asteroids")
        {
            Destroy(collision.gameObject); 
        }

        GameObject gm = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
        Destroy(gm, 2f);
        Destroy(this.gameObject); // Xoá đạn
    }
}
