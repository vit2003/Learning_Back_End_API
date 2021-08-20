using System;
using Microsoft.EntityFrameworkCore;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Persistance
{
    public class DataContext : IdentityDbContext<AppUser> 
    {
        public DataContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Value> Value { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Value>().HasData(
                new Value
                {
                Id = 1,
                Name = "Value 101"
                },   new Value
                {
                    Id = 2,
                    Name = "Value 102"
                },   new Value
                {
                    Id = 3,
                    Name = "Value 103"
                });

            //set key for UserActivity:
            builder.Entity<UserActivity>(x => x.HasKey(ua => new
            {
                ua.AppUserId,
                ua.ActivityId
            }));
            //set relasionship UserActivity with AppUser
            builder.Entity<UserActivity>()
                .HasOne(u => u.AppUser)
                .WithMany(a => a.UserActivities)
                .HasForeignKey(u => u.AppUserId);
            //set relasionship UserActivity with Activity
            builder.Entity<UserActivity>()
                .HasOne(a => a.Activity)
                .WithMany(u => u.UserActivities)
                .HasForeignKey(a => a.ActivityId);
        }
    }
}
