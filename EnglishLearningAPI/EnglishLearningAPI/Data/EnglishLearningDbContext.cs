using System;
using System.Collections.Generic;
using EnglishLearningAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


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

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<FavoriteWord> FavoriteWords { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WordHistory> WordHistories { get; set; }

	public DbSet<WordDictionary> WordDictionaries { get; set; }



	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=EnglishLearningDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Conversa__3214EC070A674FCF");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId).HasMaxLength(100);

            entity.HasOne(d => d.Topic).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conversations_Topics");
        });

        modelBuilder.Entity<FavoriteWord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Favorite__3214EC075419C709");

            entity.Property(e => e.FavoritedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId).HasMaxLength(100);
            entity.Property(e => e.Word).HasMaxLength(100);
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Topics__3214EC0759291B42");

            entity.Property(e => e.Description).HasMaxLength(2047);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(300);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.RegisteredAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SerialNo).ValueGeneratedOnAdd();
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<WordHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WordHist__3214EC0759D0C90F");

            entity.ToTable("WordHistory");

            entity.Property(e => e.ClickCount).HasDefaultValue(1);
            entity.Property(e => e.FirstViewedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastViewedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId).HasMaxLength(100);
            entity.Property(e => e.Word).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
