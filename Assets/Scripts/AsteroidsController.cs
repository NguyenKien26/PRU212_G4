using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsController : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = Object.FindAnyObjectByType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("AsteroidsController: PlayerController not found in the scene!");
        }
    }

    private void Update()
    {
        if (playerController != null)
        {
            transform.Translate(Vector3.down * playerController.AsteroidSpeed * Time.deltaTime);
        }
        else
        {
            Debug.LogWarning("AsteroidsController: PlayerController is null, cannot access asteroidSpeed.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.AsteroidDestroyed(transform.position);
                Destroy(gameObject);
                Destroy(collision.gameObject);
                Debug.Log("Thiên thạch bị phá hủy bởi đạn.");
            }
            else
            {
                Debug.LogError("GameManager instance là null.");
            }
        }
    }
}