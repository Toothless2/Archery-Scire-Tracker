using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace Archery_Performance_Tracker.JSONStuff
{
    public class JSONFile
    {
        public List<JSONScore> scores = new List<JSONScore>();

        public DateTime getOldestScore() => scores.Count > 0 ? DateTime.FromOADate(scores.OrderBy(e => e.date).First().date) : default;

        public bool addNewScore(JSONScore score)
        {
            foreach (var stored in scores.Where(stored => Math.Abs(stored.date - score.date) < 0.01f))
            {
                stored.nShots = score.nShots;
                stored.scores = stored.scores != null ? stored.scores.Concat(score.scores).OrderBy(o => o).ToArray() : score.scores; // of not null concat them
                
                return false; // false as no new item was added
            }

            scores.Add(score);

            scores = scores.OrderBy(e => e.date).ToList();
            
            return true;
        }
    }
}