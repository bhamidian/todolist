using ConsoleApp.Data.Enums;

namespace ConsoleApp.Data.Models;

public class TaskHistory
{
    public TaskHistory()
    {

    }
    public int Id { get; set; }
    public int TaskId { get; set; }
    public TaskItem TaskItem { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public string Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public DateTime ChangedAt { get; set; }  

    public static TaskHistory LogHistory(TaskItem task)
    {
        return new TaskHistory
        {
            TaskId = task.Id,
            TimeSpent = task.TimeSpent,
            Description = task.Description,
            Status = task.Status,
            ChangedAt = DateTime.Now
        };
    }
}
