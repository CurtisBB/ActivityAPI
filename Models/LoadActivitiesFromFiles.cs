using Newtonsoft.Json;

public class LoadActivitiesFromFiles {
     // Path to the directory 
    private readonly string directoryPath = @"C:\Users\curti\ActivitiesData";
    //public string directoryPath = @"C:\Users\Curtis\.Projects\ActivityAPI\Data";

    public List<Activity> ReadActivitiesFromFiles()
    {    
        List<Activity> activities = new List<Activity>();

        if (Directory.Exists(directoryPath))
        {   
            foreach (string filePath in Directory.GetFiles(directoryPath, "*.txt"))
            {
                string json = File.ReadAllText(filePath);
                Activity activity = JsonConvert.DeserializeObject<Activity>(json);                
                activities.Add(activity);
            }
        }         
        
        return activities;       
    }
}