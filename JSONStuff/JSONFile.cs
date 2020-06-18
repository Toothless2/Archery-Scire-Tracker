using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using Archery_Performance_Tracker.Enums;

namespace Archery_Performance_Tracker.JSONStuff
{
    public class JSONFile
    {
        public List<JSONScore>[] scores = new List<JSONScore>[Enum.GetNames(typeof(ERound)).Length]; // creates enough rounds

        public ref List<JSONScore> getRoundRoundScores(ERound r)
        {
            if(scores[(int)r] == null)
                scores[(int) r] = new List<JSONScore>();

            return ref scores[(int)r];
        }

        private void setScoreList(List<JSONScore> score, ERound r)
        {
            scores[(int)r] = score;
            scores[(int) r] = scores[(int) r].OrderBy(o => o.date).ToList();
        }

        public DateTime getOldestScore(ERound r)
        {
            return scores[(int)r] != null && scores[(int) r].Count > 0
                ? DateTime.FromOADate(scores[(int) r].OrderBy(e => e.date).First().date) // return the oldest if the list isnt empty
                : default;
        }

        public bool addNewScore(JSONScore score, ERound r)
        {
            var round = getRoundRoundScores(r);
            
            foreach (var stored in round.Where(stored => Math.Abs(stored.date - score.date) < 0.01f))
            {
                stored.nShots = score.nShots;
                stored.scores = score.scores;
                
                return false; // false as no new item was added
            }

            round.Add(score);
            setScoreList(round, r);
            
            return true;
        }
    }
}