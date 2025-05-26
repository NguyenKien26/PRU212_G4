using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{

    public static int life = 3;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;


    private void Awake()
    {
        life = 3;
    }
    void Update()
    {
        foreach (Image heartImg in hearts)
        {
            heartImg.sprite = emptyHeart;
        }
        for (int i = 0; i < life; i++)
        {
            hearts[i].sprite = fullHeart;
        }


    }
}
