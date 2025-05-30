using UnityEngine;

public class AsteroidsController : MonoBehaviour
{
    public float speed;
    private void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime); 
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Giả định đạn có tag "Bullet"
        if (collision.CompareTag("Bullet"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.AsteroidDestroyed(transform.position);
                Destroy(gameObject); // Hủy thiên thạch
                Destroy(collision.gameObject); // Hủy đạn
                Debug.Log("Thiên thạch bị phá hủy bởi đạn.");
            }
            else
            {
                Debug.LogError("GameManager instance là null.");
            }
        }
    }
}
