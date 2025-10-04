namespace ConsoleApp.Data.Models;

public class Comment
{
    public Comment()
    {
    }

    public Comment(string text, DateTime postedAt,int taskid,int userid )
    {
        Text = text;
        PostedAt = postedAt;
        UserId = userid;
        TaskId = taskid;
        
    }
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int TaskId { get; set; }
    public TaskItem TaskItem { get; set; }
    public string Text { get; set; }
    public DateTime PostedAt { get; set; }
}