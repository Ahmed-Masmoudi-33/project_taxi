using metiers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace taxi.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users  { get; set; }
        public DbSet<Taxi> Taxis { get; set; }
        public DbSet<Ride> Rides { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Commission> Commissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Employee)
                .WithMany(u => u.Assignments)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Taxi)
                .WithMany(t => t.Assignments)
                .HasForeignKey(a => a.TaxiId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Ride>()
    .HasOne(r => r.Taxi)
    .WithMany(t => t.Rides)
    .HasForeignKey(r => r.TaxiId)
    .OnDelete(DeleteBehavior.Cascade); // keep cascade here

            modelBuilder.Entity<Ride>()
                .HasOne(r => r.Employee)
                .WithMany(u => u.Rides)
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict); // prevent multiple cascade paths

        }

}
    }