using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


[EnableCors("testPolicy")]

[Route("api/[controller]")]

[ApiController]
public class ActivitiesController : ControllerBase
{
   private readonly string directoryPath = @"C:\Users\curti\ActivitiesData";

   [HttpPost("save-activities")]
    public IActionResult SaveActivity(Activity activity)
    {   
        if (activity.Type == ActivityType.Unknown)
        {
            ModelState.AddModelError("Type", "Activity Type must not be unkown.");
            return BadRequest(ModelState);
        }

        string activityJson = JsonConvert.SerializeObject(activity, Formatting.Indented);

        string fileName = activity.Id + ".txt";
        string filePath = Path.Combine(directoryPath, fileName);

        System.IO.File.WriteAllText(filePath, activityJson);

        return Ok(activityJson);
    } 


    [HttpGet("get-activities")]
    public IActionResult GetActivities()
    {
        LoadActivitiesFromFiles loadActivity = new LoadActivitiesFromFiles();
        List<Activity> activities = loadActivity.ReadActivitiesFromFiles();

        string printActivities = JsonConvert.SerializeObject(activities, Formatting.Indented);

        return Ok(printActivities);
    }


    [HttpPatch("update-activities/{activityId}")]
    public IActionResult UpdateActivity(Guid activityId, [FromBody] JsonPatchDocument<Activity> patchDoc)
    {   
        
        string activityFilePath = Path.Combine(directoryPath, $"{activityId}.txt");

        if (!System.IO.File.Exists(activityFilePath))
        {
            return NotFound("Activity not found.");
        }

        // Read the existing activity JSON from the file
        string existingActivityJson = System.IO.File.ReadAllText(activityFilePath);
       
        Activity updateActivity = JsonConvert.DeserializeObject<Activity>(existingActivityJson);

        patchDoc.ApplyTo(updateActivity, ModelState);        

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string updateActivityJson = JsonConvert.SerializeObject(updateActivity, Formatting.Indented);

        // Write the updated JSON back to the file
        System.IO.File.WriteAllText(activityFilePath, updateActivityJson);

        return Ok(updateActivityJson);
    } 


    [HttpGet("activity-type-summary")]
    public IActionResult GetActivityTypeSummary(DateTime startDate, DateTime endDate)
    {
        LoadActivitiesFromFiles loadActivity = new LoadActivitiesFromFiles();
        List<Activity> activities = loadActivity.ReadActivitiesFromFiles();
         

        var groupedActivities = activities
            .Where(a => a.DateTimeStarted >= startDate && a.DateTimeFinished <= endDate)
            .GroupBy(a => a.Type.ToString())
            .Select(group => new ActivitySummary
            {
                Type = group.Key,
                TotalElapsedTime = group.Sum(a => a.ElapsedTime),
                Activities = group.ToList()                
            })
            .ToList();        
        
        string groupJson = JsonConvert.SerializeObject(groupedActivities, Formatting.Indented);

        return Ok(groupJson);
    } 
}
