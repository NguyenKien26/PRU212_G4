using UnityEngine;

public class StarController : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f; // Thời gian tồn tại của ngôi sao

    private void Awake()
    {
        // Kiểm tra các thành phần cần thiết
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError("StarController: Thiếu Collider2D trên ngôi sao!");
        }
        else
        {
            GetComponent<Collider2D>().isTrigger = true; // Đảm bảo là trigger
        }

        // Hủy ngôi sao sau lifetime giây
        Destroy(gameObject, lifetime);
        Debug.Log("Ngôi sao đã được tạo.");
    }
}
