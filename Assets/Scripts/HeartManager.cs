using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static int life = 3;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;


    // Update is called once per frame
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
