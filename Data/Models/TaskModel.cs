using ConsoleApp.Data.Enums;

namespace ConsoleApp.Data.Models;

public class TaskItem
{
    public TaskItem(string description, int? assigneeId, int creatorId)
    {
        CreatorId = creatorId;
        AssigneeId = assigneeId;
        Description = description;
    }
    public TaskItem()
    {

    }
    public int Id { get; set; }
    public string Description { get; set; }
    public int CreatorId { get; set; }
    public User Creator { get; set; }
    public int? AssigneeId { get; set; }
    public User? Assignee { get; set; }
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public TimeSpan TimeSpent { get; set; }
    public TaskItemStatus Status { get; set; }
    public ICollection<TaskHistory> TaskHistories { get; set; } = new List<TaskHistory>();

}