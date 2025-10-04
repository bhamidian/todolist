using ConsoleApp.Commands;
using ConsoleApp.Data;
using ConsoleApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp;

public class Program
{
    static async Task Main(string[] args)
    {
        var options = new DbContextOptionsBuilder<TaskManagementContext>()
            .UseSqlite("Data Source=taskmanagement.db")
            .Options;
        await using var context = new TaskManagementContext(options);
        await context.Database.EnsureCreatedAsync();

        Console.WriteLine("Welcome to Task Management System");

        while (true)
        {
            Console.Write("Enter username: ");
            var username = Console.ReadLine();

            Console.Write("Enter password: ");
            var password = Console.ReadLine();

            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                Console.WriteLine($"Welcome {user.Username}! You are logged in as {user.Role}");
                await ManageTasksAsync(user, context);
                break;
            }

            Console.WriteLine("Invalid username or password. Please try again.");
        }
    }
    
    
    private static async Task ManageTasksAsync(User user, TaskManagementContext context)
    {
        if (user.Role == "Admin")
        {
            var adminCommands = new AdminCommands(context);
            await AdminMenuAsync(adminCommands);
        }
        else
        {
            var regularUserCommands = new RegularUserCommands(context, user);
            await RegularUserMenuAsync(regularUserCommands);
        }
    }

    private static async Task AdminMenuAsync(AdminCommands oldAdminCommands)
    {
        while (true)
        {
            Console.WriteLine("\nAdmin Menu:");
            Console.WriteLine("1. Create User");
            Console.WriteLine("2. List Users");
            Console.WriteLine("3. Delete User");
            Console.WriteLine("4. Create Task");
            Console.WriteLine("5. List Tasks");
            Console.WriteLine("6. Promote User to Admin");
            Console.WriteLine("7. Logout");

            Console.Write("Select an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await oldAdminCommands.CreateUserAsync();
                    break;
                case "2":
                    await oldAdminCommands.ListUsersAsync();
                    break;
                case "3":
                    await oldAdminCommands.DeleteUserAsync();
                    break;
                case "4":
                    await oldAdminCommands.CreateTaskAsync();
                    break;
                case "5":
                    await oldAdminCommands.ListTasksAsync();
                    break;
                case "6":
                    await oldAdminCommands.PromoteUserToAdminAsync();
                    break;
                case "7":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private static async Task RegularUserMenuAsync(RegularUserCommands regularUserCommands)
    {
        while (true)
        {
            Console.WriteLine("\nUser Menu:");
            Console.WriteLine("1. List My Tasks");
            Console.WriteLine("2. Update Task Status");
            Console.WriteLine("3. Update Time Spent on Task");
            Console.WriteLine("4. Create Task");
            Console.WriteLine("5. Add Comment");
            Console.WriteLine("6. Logout");

            Console.Write("Select an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await regularUserCommands.ListMyTasksAsync();
                    break;
                case "2":
                    await regularUserCommands.UpdateTaskStatusAsync();
                    break;
                case "3":
                    await regularUserCommands.UpdateTimeSpentAsync();
                    break;
                case "4":
                    await regularUserCommands.CreateTaskAsync();
                    break;
                case "5":
                    await regularUserCommands.AddCommentAsync();
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}
