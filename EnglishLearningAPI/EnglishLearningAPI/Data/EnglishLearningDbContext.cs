using System;
using System.Collections.Generic;
using EnglishLearningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Data;

public partial class EnglishLearningDbContext : DbContext
{
    public EnglishLearningDbContext(DbContextOptions<EnglishLearningDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<FavoriteWord> FavoriteWords { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WordDictionary> WordDictionaries { get; set; }

    public virtual DbSet<WordHistory> WordHistories { get; set; }

	public DbSet<InvalidWord> InvalidWords { get; set; }


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

            entity.HasOne(d => d.User).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conversations_Users");
        });

        modelBuilder.Entity<FavoriteWord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Favorite__3214EC075419C709");

            entity.Property(e => e.FavoritedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId).HasMaxLength(100);
            entity.Property(e => e.Word).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.FavoriteWords)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FavoriteWords_Users");
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

        modelBuilder.Entity<WordDictionary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WordDict__3214EC0745CE2F15");

            entity.ToTable("WordDictionary");

            entity.HasIndex(e => e.Word, "UQ__WordDict__95B501083059263D").IsUnique();

            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PartOfSpeech).HasMaxLength(100);
            entity.Property(e => e.Phonetic).HasMaxLength(100);
            entity.Property(e => e.Word).HasMaxLength(100);
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

            entity.HasOne(d => d.User).WithMany(p => p.WordHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WordHistory_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
