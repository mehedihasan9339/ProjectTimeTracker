using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProjectTimeTracker
{
    public class ProjectTimeData
    {
        public string ProjectName { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan TotalTimeSpent { get; set; }
    }

    public static class TimeDataHelper
    {
        private static string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProjectTimeTracker.json");

        public static void SaveTimeData(ProjectTimeData data)
        {
            List<ProjectTimeData> existingData = LoadTimeData();
            existingData.Add(data);

            File.WriteAllText(filePath, JsonConvert.SerializeObject(existingData));
        }

        public static List<ProjectTimeData> LoadTimeData()
        {
            if (File.Exists(filePath))
            {
                return JsonConvert.DeserializeObject<List<ProjectTimeData>>(File.ReadAllText(filePath));
            }
            return new List<ProjectTimeData>();
        }
    }
}
