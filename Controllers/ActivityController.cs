using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Route("api/[controller]")]
[ApiController]
public class ActivitiesController : ControllerBase
{
    // Path to the directory 
    public string directoryPath = "C:\\Users\\curti\\ActivitiesData";
    //public string directoryPath = "C:\\Users\\Curtis\\.Projects\\ActivityAPI\\Data";

    private JArray LoadActivitiesFromFiles()
    {   
        JArray listActivities = new JArray();

        if (Directory.Exists(directoryPath))
        {
            var fileNames =  Directory.GetFiles(directoryPath, "*.txt");

            foreach (var txtFile in fileNames)
            {
                var json = System.IO.File.ReadAllText(txtFile);
                Activity activity = JsonConvert.DeserializeObject<Activity>(json);     
                JObject activityObject = JObject.FromObject(activity);
                listActivities.Add(activityObject);       
            }        
        }         
        
        return listActivities;       
    }

    [HttpPost("save-activities")]
    public IActionResult SaveActivity(Activity activity)
    {
        // Check activity type is valid
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        activity.ElapsedTime = activity.DateTimeFinished - activity.DateTimeStarted;

        string activityJson = JsonConvert.SerializeObject(activity, Formatting.Indented);

        // Create a unique file name 
        Guid guid = Guid.NewGuid();
        string fileName = guid.ToString() + ".txt";
        string filePath = Path.Combine(directoryPath, fileName);

        System.IO.File.WriteAllText(filePath, activityJson);

        return Ok(activityJson);
    }

    [HttpGet("get-activities")]
    public IActionResult GetActivities()
    {
        var activities = LoadActivitiesFromFiles();

        string printActivities = JsonConvert.SerializeObject(activities, Formatting.Indented);

        return Ok(printActivities);
    }


    [HttpPut("{activityId}")]
    public IActionResult UpdateActivity(Guid activityId, [FromBody] Activity updatedActivity)
    {   
        
        var activityFilePath = Path.Combine(directoryPath, $"{activityId}.txt");

        if (!System.IO.File.Exists(activityFilePath))
        {
            return NotFound("Activity not found.");
        }

        // Read the existing activity JSON from the file
        string existingActivityJson = System.IO.File.ReadAllText(activityFilePath);
        Activity existingActivity = JsonConvert.DeserializeObject<Activity>(existingActivityJson);

        // Apply updates to existingActivity based on updatedActivity
        existingActivity.Name = updatedActivity.Name;
        existingActivity.DateTimeStarted = updatedActivity.DateTimeStarted;
        existingActivity.DateTimeFinished = updatedActivity.DateTimeFinished;

        // Update elapsed time
        updatedActivity.ElapsedTime = updatedActivity.DateTimeFinished - updatedActivity.DateTimeStarted;
        
        string updatedActivityJson = JsonConvert.SerializeObject(updatedActivity, Formatting.Indented);

        // Write the updated JSON back to the file
        System.IO.File.WriteAllText(activityFilePath, updatedActivityJson);

        return Ok(updatedActivityJson);
    } 


    [HttpGet("activity-type-summary")]
    public IActionResult GetActivityTypeSummary(DateTime startDate, DateTime endDate)
    {
        var activities = LoadActivitiesFromFiles(); 

        var groupedActivities = activities
            .Where(a => (DateTime)a["DateTimeStarted"] >= startDate && (DateTime)a["DateTimeFinished"] <= endDate)
            .GroupBy(a => a["Type".ToString()])
            .Select(group => new ActivitySummary
            {
                Type = (string)group.Key,
                TotalElapsedTime = TimeSpan.FromTicks(group.Sum(a => ((TimeSpan)a["ElapsedTime"]).Ticks)),
                Activities = group.Select(a => a.ToObject<Activity>()).ToList()                
            })
            .ToList();        
        
        string groupJson = JsonConvert.SerializeObject(groupedActivities, Formatting.Indented);

        return Ok(groupJson);
    }
}
