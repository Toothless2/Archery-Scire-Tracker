#nullable enable
using System;

namespace Archery_Performance_Tracker.JSONStuff
{
    public class JSONScore
    {
        public double date;
        public int nShots;
        public float[] scores;

        public JSONScore(double date, int nShots, float[]? scores)
        {
            this.date = date;
            this.nShots = nShots;
            this.scores = scores;
        }
    }
}