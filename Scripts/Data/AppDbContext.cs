using Microsoft.EntityFrameworkCore;

namespace GameBackend.Scripts.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}