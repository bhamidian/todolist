namespace ConsoleApp.Data.Models;

public class User
{

    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public List<Comment> Comments { get; set; } = new List<Comment>();
}