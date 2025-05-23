using UnityEngine;

public class AsteroidsController : MonoBehaviour
{
    public float speed;
    private void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime); 
    }
}
