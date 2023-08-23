using System.ComponentModel.DataAnnotations;

public class Activity
{
    public int Id { get; set; }
    public int FirmId { get; set; }
    public string Name { get; set; }

    [Required]
    [AllowedActivityTypes]
    public string Type { get; set; }
    public DateTime DateTimeStarted { get; set; }
    public DateTime DateTimeFinished { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    
    
    public List<Attachment>? Attachments { get; set; }
}

public class Attachment 
{
    public string Id { get; set; }
    public string Name { get; set; }
}