using ConsoleApp.Data;
using ConsoleApp.Data.Enums;
using ConsoleApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp.Commands;

public class RegularUserCommands
{
    private readonly TaskManagementContext _context;
    private readonly User _currentUser;

    public RegularUserCommands(TaskManagementContext context, User currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ListMyTasksAsync()
    {

        var tasks = await _context.Tasks
            .Where(x => _currentUser.Id == x.AssigneeId)
            .Include(x => x.Comments)
            .Include(x => x.TaskHistories)
            .ToListAsync();

        foreach (var task in tasks)
        {
            Console.WriteLine($"{task.Id}: {task.Description}, Status: {task.Status}");
            await PrintTaskComments(task);

        }

    }

    public async Task UpdateTaskStatusAsync()
    {
        if (!int.TryParse(Console.ReadLine(), out int taskid))
        {
            Console.WriteLine("Invalid input.");
            return;
        }

        var task = await _context.Tasks
            .FirstOrDefaultAsync(x => x.AssigneeId == _currentUser.Id && x.Id == taskid);
        if (task == null)
        {
            Console.WriteLine("Task not found or you do not have permission to update this task.");
            return;
        }

        string? inputString = Console.ReadLine();
        if (Enum.TryParse<TaskItemStatus>(inputString, ignoreCase: true, out var status))
        {
            try
            {
                task.Status = status;
                await _context.SaveChangesAsync();

                var history = TaskHistory.LogHistory(task);
                history.TaskItem = task; 

                await _context.TaskHistories.AddAsync(history);
                await _context.SaveChangesAsync();

                Console.WriteLine("Task status updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                Console.WriteLine("try again!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Invalid status.");
        }
    }


    public async Task CreateTaskAsync()
    {
        string? Description = Console.ReadLine();
        if (string.IsNullOrEmpty(Description) || string.IsNullOrWhiteSpace(Description))
        {
            Console.WriteLine("Description cannot be empty.");
            return;
        }

        string? assigneeInput = Console.ReadLine();
        if (!int.TryParse(assigneeInput, out int assigneeid))
        {
            Console.WriteLine("Invalid assignee ID.");
            return;
        }

        var person = await _context.Users.FirstOrDefaultAsync(x => x.Id == assigneeid);
        if (person == null)
        {
            Console.WriteLine("User not found or you do not have permission to create this task.");
            return;
        }

        try
        {
            var newtask = new TaskItem(Description, assigneeid, _currentUser.Id);

            await _context.Tasks.AddAsync(newtask);

            var history = TaskHistory.LogHistory(newtask);
            history.TaskItem = newtask; 

            _context.TaskHistories.Add(history);
            await _context.SaveChangesAsync();

            Console.WriteLine("Task created and assigned successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }



    public async Task AddCommentAsync()
    {
        if (int.TryParse(Console.ReadLine(), out int taskid))
        {

            var task = await _context.Tasks
                .FirstOrDefaultAsync(x => x.CreatorId == _currentUser.Id ||
                x.AssigneeId == _currentUser.Id && x.Id == taskid);

            if (task == null)
            {
                Console.WriteLine("Task not found or you do not have permission to update this task.");
            }
            else
            {
                string? comment = Console.ReadLine();
                if (string.IsNullOrEmpty(comment) || string.IsNullOrWhiteSpace(comment))
                {
                    Console.WriteLine("Comment cannot be empty.");
                    return;
                }
                else
                {
                    try
                    {
                        var newcomment = new Comment(comment, DateTime.Now, taskid,_currentUser.Id);
                        task.Comments.Add(newcomment);
                        await _context.SaveChangesAsync();
                        Console.WriteLine("Comment added successfully.");
                        

                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                    }

                }

            }

        }
        else
        {
            Console.WriteLine("Invalid task ID.");

        }
    }

    public async Task UpdateTimeSpentAsync()
    {
        if (!int.TryParse(Console.ReadLine(), out int taskid))
        {
            Console.WriteLine("Invalid task ID.");
            return;
        }

        var task = await _context.Tasks
            .FirstOrDefaultAsync(x => x.AssigneeId == _currentUser.Id && x.Id == taskid);
        if (task == null)
        {
            Console.WriteLine("Task not found or you do not have permission to update this task.");
            return;
        }

        if (!double.TryParse(Console.ReadLine(), out double timesp))
        {
            Console.WriteLine("Invalid time format.");
            return;
        }

        try
        {
            task.TimeSpent = TimeSpan.FromHours(timesp);
            await _context.SaveChangesAsync();

            var history = TaskHistory.LogHistory(task);
            history.TaskItem = task; 

            await _context.TaskHistories.AddAsync(history);
            await _context.SaveChangesAsync();

            Console.WriteLine("Task time spent updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }


    public async Task PrintTaskComments(TaskItem task)
    {
        var comments = task.Comments;

        foreach (var comment in comments)
        {
            Console.WriteLine($"- [{comment.PostedAt}] {comment.User?.Username ?? "Unknown"}: {comment.Text}");
        }

    }
}