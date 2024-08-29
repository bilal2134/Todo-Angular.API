using Microsoft.EntityFrameworkCore;

namespace TodoApp.API
{
    public class TodoDbContext : DbContext
    {
        public DbSet<Todo> Todos { get; set; }

        public TodoDbContext(DbContextOptions<TodoDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Todo>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("uuid");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}