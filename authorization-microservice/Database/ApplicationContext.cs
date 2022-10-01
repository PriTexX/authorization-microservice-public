using authorization_microservice.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace authorization_microservice.Database;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public ApplicationContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
    }
}