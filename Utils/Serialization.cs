#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
using Archery_Performance_Tracker.Enums;
using Archery_Performance_Tracker.JSONStuff;
using Newtonsoft.Json;

namespace Archery_Performance_Tracker.Utils
{
    public static class Serialization
    {
        private static string filePath => Environment.CurrentDirectory;

        public static JSONFile data
        {
            get;
            private set;
        } = new JSONFile(); // default value
    
        private static string jsonString = "";
        
        public static JSONFile loadScores()
        {
            if (!File.Exists($"{filePath}\\SavedScore.json"))
                return null;

            jsonString = File.ReadAllText($"{filePath}\\SavedScore.json");

            data = JsonConvert.DeserializeObject<JSONFile>(jsonString);

            return data;
        }

        public static JSONScore getScore(ERound r, int roundType)
        {
            var d = data.getRoundRoundScores(r);
            return d.Count > roundType ? d[roundType] : null;
        }
        
        public static void saveShots(double date, int nShot)
        {
            data.addShots(date, nShot);
            serializeAndSave();
        }
        
        public static bool saveScores(double date, float[]? scores, ERound round)
        {
            //update the data correctly
            var v = data.addNewScore(new JSONScore(date, scores), round);
            
            serializeAndSave();

            return v;
        }

        public static void deletePoint(double date, ERound round)
        {
            var s = data.scores[(int) round].FirstOrDefault(e => Math.Abs(e.date - date) < 0.01f);
            var shots = data.shots.IndexOf(data.shots.FirstOrDefault(e => Math.Abs(e.date - date) < 0.01f));

            if (s != null)
                data.scores[(int) round].Remove(s);
            
            if(shots != -1)
                data.shots.RemoveAt(shots);
            
            serializeAndSave();
        }
        
        private static void serializeAndSave()
        {
            jsonString = JsonConvert.SerializeObject(data);
            
            File.WriteAllText($"{filePath}\\SavedScore.json", jsonString);
        }

        public static int getShots(double scoreDate) => data.shots.FirstOrDefault(e => Math.Abs(e.date - scoreDate) < 0.01f).shots;

        public static (double date, int shots) getShotsFromIndex(int currentSelected)
        {
            if (data.shots != null && data.shots.Count > currentSelected)
                return data.shots[currentSelected];
            
            return default;
        }
    }
}