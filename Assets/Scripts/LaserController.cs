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
        if (collision.gameObject.tag == "Asteroids" || collision.gameObject.tag == "Enemy")
        {
            if (collision.gameObject.CompareTag("Asteroids") || collision.gameObject.CompareTag("Enemy"))
            {
                if (GameManager.instance != null && GameManager.instance.explosion != null)
                {
                    GameObject gm = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
                    Destroy(gm, 2f);
                }

                // Nếu là Enemy thì gọi DropLoot
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.DropLoot();
                    }
                }

                Destroy(this.gameObject);             // Hủy laser
                Destroy(collision.gameObject);        // Hủy enemy
            }
        }
    }
}
