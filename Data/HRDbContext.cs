using System;
using HRManagement.Models;
using HRManagement.Models.Orgnisations;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Data;

public class HRDbContext : DbContext
{
    public HRDbContext(DbContextOptions<HRDbContext> options) : base(options)
    {

    }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    
    public DbSet<RefreshToken> RefreshTokens{get;set;}
    public DbSet<Organisation> Organisations{get;set;}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new {ur.UserId,ur.RoleId});

        modelBuilder.Entity<UserRole>()
            .HasOne(ur=>ur.User)
            .WithMany(u=>u.UserRoles)
            .HasForeignKey(ur=>ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(u=>u.UserRoles)
            .HasForeignKey(ur =>ur.RoleId);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasOne(u => u.Organisation)
            .WithMany(o => o.Users)
            .HasForeignKey(u => u.OrganisationId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
