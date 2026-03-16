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
    }
}