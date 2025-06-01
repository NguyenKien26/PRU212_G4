using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    using UnityEngine;
    using UnityEngine.UI;

    public class ScoreUI : MonoBehaviour
    {
        public Text scoreText; // Gán từ Inspector
        private int score = 0;

        private void Start()
        {
            UpdateScoreText();
        }

        public void AddScore(int amount)
        {
            score += amount;
            UpdateScoreText();
        }

        private void UpdateScoreText()
        {
            scoreText.text = $"Score: {score}";
            Debug.Log($"Updated Score: {score}");
        }
    }

}
