using System;
using System.Collections.Generic;
using EnglishLearningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Data;

public partial class EnglishLearningDbContext : DbContext
{
    public EnglishLearningDbContext()
    {
    }

    public EnglishLearningDbContext(DbContextOptions<EnglishLearningDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=EnglishLearningDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Topics__3214EC075F1BBBA7");

            entity.Property(e => e.Description).HasMaxLength(2047);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07903607F0");

            entity.Property(e => e.Email).HasMaxLength(300);
            entity.Property(e => e.Level).HasDefaultValue(0);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
