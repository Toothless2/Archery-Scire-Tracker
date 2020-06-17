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

        public static bool saveScores(double date, int nShot, float[]? scores)
        {
            //update the data correctly
            var v = data.addNewScore(new JSONScore(date, nShot, scores));
            
            new Thread(serializeAndSave).Start();

            return v;
        }
        
        private static void serializeAndSave()
        {
            jsonString = JsonConvert.SerializeObject(data);
            
            File.WriteAllText($"{filePath}\\SavedScore.json", jsonString);
        }
        
    }
}