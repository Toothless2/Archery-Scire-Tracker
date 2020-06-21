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

            while (saving) { } // spin lock to prevent loading whilst saving

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
            
            new Thread(serializeAndSave).Start();

            return v;
        }

        public static void deleteScore(int scoreIndex, ERound round)
        {
            data.scores[(int) round].RemoveAt(scoreIndex);
            new Thread(serializeAndSave).Start();
        }

        public static void deleteShot(int shotIndex)
        {
            data.shots.RemoveAt(shotIndex);
            new Thread(serializeAndSave).Start();
        }

        private static bool saving = false;
        private static void serializeAndSave()
        {
            while (saving) // spin lock in case something else is saving
            { }

            saving = true;
            jsonString = JsonConvert.SerializeObject(data);

            File.WriteAllText($"{filePath}\\SavedScore.json", jsonString);
            saving = false;
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