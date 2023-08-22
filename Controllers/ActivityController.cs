using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ActivitiesController : ControllerBase
{
    // Path to the directory 
    private string directoryPath = "C:\\Users\\curti\\ActivitiesData";

    private List<Activity> LoadActivitiesFromXmlFiles(string directoryPath)
    {
        List<Activity> activities = new List<Activity>();
    
        if (Directory.Exists(directoryPath))
        {
            var xmlFileNames = Directory.GetFiles(directoryPath, "*.xml");

            var serializer = new XmlSerializer(typeof(Activity));

            foreach (var xmlFileName in xmlFileNames)
            {
                using (var reader = new StreamReader(xmlFileName))
                {
                    var activity = (Activity)serializer.Deserialize(reader);
                    activities.Add(activity);
                }
            }
        }

        return activities;
    }


    [HttpPost]
    public IActionResult SaveActivity(Activity activity)
    {
        // Check activity type is valid
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }


        // Calculate the elapsed time from DateTimeStarted and DateTimeFinished
        activity.ElapsedTime = activity.DateTimeFinished - activity.DateTimeStarted;

        // Serialize the activity object to XML
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Activity));

        // Ensure the directory exists
        Directory.CreateDirectory(directoryPath);

        // Create a unique file name based on the current timestamp
        Guid guid = Guid.NewGuid();
        string fileName = guid.ToString() + ".xml";
        string filePath = Path.Combine(directoryPath, fileName);

        // Serialize the activity and write it to the XML file
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            xmlSerializer.Serialize(fileStream, activity);
        }

        return Ok(activity);
    }

    [HttpPut("{activityId}")]
    public IActionResult UpdateActivity(Guid activityId, [FromBody] Activity updatedActivity)
    {   
        

        var activityFilePath = Path.Combine(directoryPath, $"{activityId}.xml");

        if (!System.IO.File.Exists(activityFilePath))
        {
            return NotFound("Activity not found.");
        }

        XmlSerializer serializer = new XmlSerializer(typeof(Activity));
        Activity existingActivity;

        using(var reader = new StreamReader(activityFilePath))
        {
            existingActivity = (Activity)serializer.Deserialize(reader);
        }

        // Apply updates to existingActivity based on updatedActivity
        existingActivity.Name = updatedActivity.Name;
        existingActivity.DateTimeStarted = updatedActivity.DateTimeStarted;
        existingActivity.DateTimeFinished = updatedActivity.DateTimeFinished;

        // Update elapsed time
        updatedActivity.ElapsedTime = updatedActivity.DateTimeFinished - updatedActivity.DateTimeStarted;

        using(var writer = new StreamWriter(activityFilePath))
        {
            serializer.Serialize(writer, updatedActivity);
        }
        
        return Ok(updatedActivity);
    }
    
  

    [HttpGet("activity-type-summary")]
    public IActionResult GetActivityTypeSummary(DateTime startDate, DateTime endDate)
    {
        var activities = LoadActivitiesFromXmlFiles(directoryPath); 
    
        var summary = activities
            .Where(a => a.DateTimeStarted >= startDate && a.DateTimeFinished <= endDate)
            .GroupBy(a => a.Type)
            .Select(group => new ActivitySummary
            {
                Type = group.Key,
                TotalElapsedTime = TimeSpan.FromTicks(group.Sum(a => a.ElapsedTime.Ticks)),
                Activities = group.ToList()
            })
            .ToList();

        return Ok(summary);
    }

    
}
