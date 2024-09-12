using Microsoft.EntityFrameworkCore;
using ToDoList.Shared.Models;

namespace ToDoListApi.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<ToDoListModel> ToDoLists { get; set; }
        public DbSet<ToDoItem> ToDoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ToDoLists)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ToDoListModel>()
                .HasMany(t => t.Items)
                .WithOne(i => i.ToDoListModel)
                .HasForeignKey(i => i.ToDoListModelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
