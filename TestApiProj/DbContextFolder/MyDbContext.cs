using Microsoft.EntityFrameworkCore;
using TestApiProj.MainEntity;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Items> Items { get; set; }

}
