using System;
 using System.Collections.Generic;
 using System.Linq;
 
 namespace Archery_Performance_Tracker.Utils
 {
     public static class Utils
     {
         public static float[] convertScoresToStringScores(string[] s)
         {
             return s.Select(float.Parse).ToArray();
         }
         public static float[] convertScoresToStringScoresSorted(string[] s)
         {
             return convertScoresToStringScores(s).OrderBy(o => o).ToArray();
         }

         public static float calcualteMean(float[] values)
         {
             return values.Sum() / values.Length;
         }
     }
 }