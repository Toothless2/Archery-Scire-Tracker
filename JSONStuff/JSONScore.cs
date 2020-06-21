#nullable enable
using System;

namespace Archery_Performance_Tracker.JSONStuff
{
    public class JSONScore
    {
        public double date;
        public float[]? scores;

        public JSONScore(double date, float[]? scores)
        {
            this.date = date;
            this.scores = scores;
        }
    }
}