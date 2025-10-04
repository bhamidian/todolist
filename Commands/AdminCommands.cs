using ConsoleApp.Data;
using ConsoleApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ConsoleApp.Commands;

public class AdminCommands
{
    private readonly TaskManagementContext _context;

    public AdminCommands(TaskManagementContext context)
    {
        _context = context;
    }

    public async Task CreateUserAsync()
    {
        string? username = Console.ReadLine();
        string? password = Console.ReadLine();
        string? role = Console.ReadLine();



        if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("Username cannot be empty.");
            return;
        }

        else if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Password cannot be empty.");
            return;
        }

        else if (role != "Admin" && role != "Regular")
        {
            Console.WriteLine("Invalid role.");
            return;
        }
        else
        {
            try
            {
                var user = new User()
                {
                    Username = username,
                    Password = password,
                    Role = role
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                Console.WriteLine("User created successfully.");


            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


    }

    public async Task ListUsersAsync()
    {
        var users = await _context.Users.ToListAsync();

        foreach(var user in users)
        {
            Console.WriteLine($"ID: {user.Id}, Username: {user.Username}, Role: {user.Role}");
        }

    }

    public async Task DeleteUserAsync()
    {

        if (int.TryParse(Console.ReadLine(), out int userid))
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userid);

            if (user is null)
            {
                Console.WriteLine("User not found.");
                return;
               
            }
            else
            {
                try
                {
                    _context.Remove(user);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("User deleted successfully.");


                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid input.");
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

        string? creatorInput = Console.ReadLine();
        if (!int.TryParse(creatorInput, out int creatorid))
        {
            Console.WriteLine("Invalid creator ID.");
            return;
        }

        var creator = await _context.Users.FirstOrDefaultAsync(x => x.Id == creatorid);
        if (creator is null)
        {
            Console.WriteLine("Creator ID does not exist.");
            return;
        }

        string? assigneeInput = Console.ReadLine();
        int? assigneeid = null;

        if (!string.IsNullOrWhiteSpace(assigneeInput))
        {
            if (int.TryParse(assigneeInput, out int parsedAssigneeId))
            {
                var assignee = await _context.Users.FirstOrDefaultAsync(x => x.Id == parsedAssigneeId);
                if (assignee is null)
                {
                    Console.WriteLine("Assignee ID does not exist.");
                    return;
                }
                assigneeid = parsedAssigneeId;
            }
            else
            {
                Console.WriteLine("Invalid assignee ID.");
                return;
            }
        }

        try
        {
            var newtask = new TaskItem(Description, assigneeid, creatorid);

            await _context.Tasks.AddAsync(newtask);
            await _context.SaveChangesAsync();

            var history = TaskHistory.LogHistory(newtask);
            history.TaskItem = newtask;

            await _context.TaskHistories.AddAsync(history);
            await _context.SaveChangesAsync();

            Console.WriteLine("Task created successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }




    public async Task ListTasksAsync()
    {
        var tasks = await _context.Tasks
                    .Include(t => t.Assignee)
                    .Include(t => t.TaskHistories)
                    .ToListAsync();

        foreach (var task in tasks)
        {
            Console.WriteLine($"{task.Id}: {task.Description}, Status: {task.Status}, Assignee: {task.Assignee?.Username}");

        }

    }

    public async Task PromoteUserToAdminAsync()
    {
        if (int.TryParse(Console.ReadLine(), out int userid))
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userid);

            if (user == null)
            {
                Console.WriteLine("User not found.");
                return;
            }
            else
            {
                if (user.Role == "Admin")
                {
                    Console.WriteLine("User is already an Admin.");
                    return;
                }
                else if(user.Role == "Regular")

                {
                    try
                    {
                        user.Role = "Admin";
                        await _context.SaveChangesAsync();
                        Console.WriteLine("User promoted to Admin successfully.");
                        return;



                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }


                }
                else
                {
                    Console.WriteLine("User has an invalid role. Promotion cannot proceed.");
                    return;
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid input.");
            return;
        }
    }
}