using ConsoleApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure; // Add this using directive

namespace ConsoleApp.Data;

public class TaskManagementContext : DbContext
{
    public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<TaskHistory> TaskHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<User>()
            .HasMany(u => u.Tasks)
            .WithOne(t => t.Assignee)
            .HasForeignKey(t => t.AssigneeId);



        modelBuilder.Entity<User>()
            .HasMany(u => u.Comments)
            .WithOne(t => t.User)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<Comment>()
            .HasOne(u => u.TaskItem)
            .WithMany(u => u.Comments)
            .HasForeignKey(a => a.TaskId);




        




        // initializing data
        // ====================================== DON'T CHANGE THIS PART ======================================
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "admin", Password = "admin", Role = "Admin" },
            new User { Id = 2, Username = "user1", Password = "user1password", Role = "Regular" }
        );
        // ====================================================================================================
        
    }
}