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
        public List<JSONScore> scores18m30cm = new List<JSONScore>();
        public List<JSONScore> scores18m60cm = new List<JSONScore>();

        public ref List<JSONScore> getRoundRoundScores(ERound r)
        {
            switch (r)
            {
                case ERound.ROUND_18M_30CM:
                    return ref scores18m30cm;
                case ERound.ROUND_18M_60CM:
                    return ref scores18m60cm;
                default:
                    throw new ArgumentOutOfRangeException(nameof(r), r, null);
            }
        }

        public DateTime getOldestScore(ERound r)
        {
            return r switch
            {
                ERound.ROUND_18M_30CM =>
                    (scores18m30cm.Count > 0 ? DateTime.FromOADate(scores18m30cm.OrderBy(e => e.date).First().date) : default),
                ERound.ROUND_18M_60CM =>
                    (scores18m60cm.Count > 0 ? DateTime.FromOADate(scores18m30cm.OrderBy(e => e.date).First().date) : default),
                _ => throw new ArgumentOutOfRangeException(nameof(r), r, null)
            };
        }

        public bool addNewScore(JSONScore score, ERound r)
        {
            var round = getRoundRoundScores(r);
            
            foreach (var stored in round.Where(stored => Math.Abs(stored.date - score.date) < 0.01f))
            {
                stored.nShots = score.nShots;
                stored.scores = stored.scores != null ? stored.scores.Concat(score.scores).OrderBy(o => o).ToArray() : score.scores; // of not null concat them
                
                return false; // false as no new item was added
            }

            round.Add(score);

            round = round.OrderBy(e => e.date).ToList();
            
            return true;
        }
    }
}