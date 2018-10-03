
namespace DatingApp.API.Data
{
    using DatingApp.API.Models;
    using Microsoft.EntityFrameworkCore;

    public class Datacontext : DbContext
    {
        public Datacontext(DbContextOptions<Datacontext> options)
        :base(options)
        {
            
        }

        public DbSet<User> Users {get;set;}

        public DbSet<Value> Values {get;set;}

        public DbSet<Photo> Photos { get; set;}
    }
}