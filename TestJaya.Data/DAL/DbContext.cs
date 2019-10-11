using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TestJaya.Data.Models;

namespace TestJaya.Data.DAL
{
    [ExcludeFromCodeCoverage]
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbContext() : base()
        {

        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = "127.0.0.1",
                InitialCatalog = "",
                UserID = "",
                Password = ""
            };
            optionsBuilder.UseLazyLoadingProxies();

            optionsBuilder.UseSqlServer(sqlConnectionStringBuilder.ConnectionString);
            base.OnConfiguring(optionsBuilder);


        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserRoom>().HasKey(ur => new { ur.Id, ur.UserID, ur.RoomID });
            modelBuilder.Entity<UserRoom>()
                .HasOne<User>(ur => ur.User)
                .WithMany(s => s.UserRooms)
                .HasForeignKey(sc => sc.UserID);


            modelBuilder.Entity<UserRoom>()
                .HasOne<Room>(sc => sc.Room)
                .WithMany(s => s.UserRooms)
                .HasForeignKey(sc => sc.RoomID);


            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }

        public override int SaveChanges()
        {
            var entities = (from entry in ChangeTracker.Entries()
                            where entry.State == EntityState.Modified || entry.State == EntityState.Added
                            select entry.Entity);

            var validationResults = new List<ValidationResult>();
            foreach (var entity in entities)
            {
                if (!Validator.TryValidateObject(entity, new ValidationContext(entity), validationResults))
                {
                    throw new ValidationException();
                }
            }
            return base.SaveChanges();
        }

    }
}
