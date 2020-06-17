#nullable enable
using System;
using System.IO;
using System.Threading;
using Archery_Performance_Tracker.JSONStuff;
using Newtonsoft.Json;

namespace Archery_Performance_Tracker.Utils
{
    public static class Serialization
    {
        private static string filePath => Environment.CurrentDirectory;

        private static JSONFile data = new JSONFile();
        private static string jsonString = "";
        
        public static JSONFile loadScores()
        {
            if (!File.Exists($"{filePath}\\SavedScore.json"))
                return null;

            jsonString = File.ReadAllText($"{filePath}\\SavedScore.json");

            data = JsonConvert.DeserializeObject<JSONFile>(jsonString);

            return data;
        }

        public static void saveScores(double date, int nShot, float[]? scores)
        {
            data.scores.Add(new JSONScore(date, nShot, scores));

            new Thread(() => serializeAndSave()).Start();
        }
        
        private static void serializeAndSave()
        {
            jsonString = JsonConvert.SerializeObject(data);
            
            File.WriteAllText($"{filePath}\\SavedScore.json", jsonString);
        }
        
    }
}