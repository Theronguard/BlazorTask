using Microsoft.EntityFrameworkCore;
using EBZShared.Models;

namespace EBZWebApi.DataAccess
{
    public class UserDA : DbContext
    {
        public DbSet<User> Users { get; set; }

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string path = Path.Join(Settings.DBPath, "users.db");
            options.UseSqlite($"Data Source={path}");
        }

        /// <summary>
        /// Sets the username as unique
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique(true);
        }

        #endregion
    }
}
