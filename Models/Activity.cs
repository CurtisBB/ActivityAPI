using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

public class Activity
{
    public Guid Id { get; set; }
    public Guid FirmId { get; set; }
    public string Name { get; set; }

    public ActivityType Type { get; set; }
    public DateTime DateTimeStarted { get; set; }
    public DateTime DateTimeFinished { get; set; }
    public int ElapsedTime { get; set; }
    
    public List<Attachment>? Attachments { get; set; }

    public bool ShouldSerializeAttachments()
    {   
        return Type == ActivityType.Email;
    }

}

public class Attachment 
{
    public string Id { get; set; }
    public string Name { get; set; }
}