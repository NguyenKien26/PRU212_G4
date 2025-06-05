using UnityEngine;

public class StarController : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f; 

    private void Awake()
    {
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError("StarController: Thiếu Collider2D trên ngôi sao!");
        }
        else
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        Destroy(gameObject, lifetime);
        Debug.Log("Ngôi sao đã được tạo.");
    }
}
