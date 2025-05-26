using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public GameObject player;
    public TextMeshProUGUI scoreText;

    void Update()
    {
        int score = player.GetComponent<PlayerController>().score;
        scoreText.text = "SCORE: " + score.ToString();
    }
}
