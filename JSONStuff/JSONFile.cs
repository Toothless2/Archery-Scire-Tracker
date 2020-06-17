using System;
using System.Collections.Generic;
using System.Linq;

namespace Archery_Performance_Tracker.JSONStuff
{
    public class JSONFile
    {
        public List<JSONScore> scores = new List<JSONScore>();

        public DateTime getOldestScore() => DateTime.FromOADate(scores.OrderBy(e => e.date).First().date);
    }
}