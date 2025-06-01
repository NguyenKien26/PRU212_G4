//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Assets.Scripts
//{
//    using System.IO;
//    using UnityEngine;

//    public class ScoreManager : MonoBehaviour
//    {
//        private int finalScore;
//        private string filePath;

//        public void SaveScore(string username, int point, string level)
//        {
//            filePath = $"{Application.persistentDataPath}/{username}.json";

//            // Check if the file exists to compare scores
//            if (File.Exists(filePath))
//            {
//                string json = File.ReadAllText(filePath);
//                PlayerData existingData = JsonUtility.FromJson<PlayerData>(json);

//                if (point <= existingData.point)
//                {
//                    Debug.Log($"New score ({point}) is not higher than the previous score ({existingData.point}). Score not saved!");
//                    return; // Do not save if the new score is not higher
//                }
//            }

//            // Save new high score
//            PlayerData newData = new PlayerData { username = username, point = point, level = level };
//            string newJson = JsonUtility.ToJson(newData, true);
//            File.WriteAllText(filePath, newJson);
//            Debug.Log($" New high score saved for {username}: {point} points ({level})");
//        }

//        public void SetFinalScore(int score)
//        {
//            finalScore = score;
//        }
//    }


//}
