using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    [System.Serializable]
    public class HighScoreData
    {
        public List<LevelScore> Scores = new List<LevelScore>();

        [System.Serializable]
        public class LevelScore
        {
            public string Username;
            public int Level;
            public int Score;
        }
    }
}