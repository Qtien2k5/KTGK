using Microsoft.EntityFrameworkCore;
using KTGK.Models;
using System.Collections.Generic;

namespace KTGK.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Exam> Exams { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Answer> Answers { get; set; }

        public DbSet<Result> Results { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<ResultDetail> ResultDetails { get; set; }

        public DbSet<Passage> Passages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔥 FIX 1: ResultDetail → Result
            modelBuilder.Entity<ResultDetail>()
                .HasOne(rd => rd.Result)
                .WithMany()
                .HasForeignKey(rd => rd.ResultId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔥 FIX 2: ResultDetail → Question
            modelBuilder.Entity<ResultDetail>()
                .HasOne(rd => rd.Question)
                .WithMany()
                .HasForeignKey(rd => rd.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔥 FIX 3: Answer → Question (QUAN TRỌNG)
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}