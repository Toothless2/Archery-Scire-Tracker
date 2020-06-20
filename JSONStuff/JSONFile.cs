using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Windows.Forms;
using Archery_Performance_Tracker.Enums;

namespace Archery_Performance_Tracker.JSONStuff
{
    public class JSONFile
    {
        public List<(double date, int shots)> shots = new List<(double, int)>();
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

        public DateTime getOldestArrowCount()
        {
            return shots != null ? DateTime.FromOADate(shots.First().date) : DateTime.Now;
        }
        
        public DateTime getOldestScore(ERound r)
        {
            if (scores.Length <= (int) r)
            {
                var b = scores.ToList();
                b.Add(new List<JSONScore>());
                scores = b.ToArray();
            }

            return scores[(int)r] != null && scores[(int) r].Count > 0
                ? DateTime.FromOADate(scores[(int) r].OrderBy(e => e.date).First().date) // return the oldest if the list isnt empty
                : DateTime.Now;
        }

        public bool addNewScore(JSONScore score, ERound r)
        {
            var round = getRoundRoundScores(r);
            
            foreach (var stored in round.Where(stored => Math.Abs(stored.date - score.date) < 0.01f))
            {
                stored.scores = score.scores;
                
                return false; // false as no new item was added
            }

            round.Add(score);
            setScoreList(round, r);
            
            return true;
        }

        public void addShots(double date, int shots)
        {
            if (this.shots == null)
                this.shots = new List<(double date, int shots)>();

            var d = this.shots.IndexOf(this.shots.FirstOrDefault(e => Math.Abs(e.date - date) < 0.01f));

            if (d != -1)
                this.shots.RemoveAt(d);
         
            this.shots.Add((date, shots));

            this.shots = this.shots.OrderBy(e => e.date).ToList();
        }
    }
}