﻿using EntityFrameworkCore.Data.Configurations;
using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EntityFrameworkCore.Data
{
    public class FootballLeagueDbContext : DbContext
    {
        //public FootballLeagueDbContext()
        //{
        //    var folder = Environment.SpecialFolder.LocalApplicationData;
        //    var path = Environment.GetFolderPath(folder);
        //    DBPath = Path.Combine(path, "FootbalLeague_EFCore.db");
        //}

        public FootballLeagueDbContext(DbContextOptions<FootballLeagueDbContext> options) : base(options)
        {

        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<TeamsAndLeaguesView> TeamsAndLeaguesView { get; set; }
        public string DBPath { get; private set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=FootbalLeague_EFCore; Encrypt=False")
        //        .UseLazyLoadingProxies() // For use Lazy Loading after install ef.Proxie package ,decommnet only if you wnat lazy loading(not recommended)
        //        //optionsBuilder.UseSqlite($"Data Source={DBPath}")
        //        //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) // for global no tracking
        //        .LogTo(Console.WriteLine, LogLevel.Information)
        //        //Not in productions
        //        .EnableSensitiveDataLogging()
        //        .EnableDetailedErrors();
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new TeamConfiguration());
            //modelBuilder.ApplyConfiguration(new LeagueConfiguration());
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<TeamsAndLeaguesView>().HasNoKey().ToView("vw_TeamsAndLeagues");
            modelBuilder.HasDbFunction(typeof(FootballLeagueDbContext).GetMethod(nameof(GetEarliestTeamMatch), new[] { typeof(int) }))
                .HasName("fn_GetEarliestMatch");
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseDomainModel>().Where(q => q.State == EntityState.Modified ||
                q.State == EntityState.Added);
            foreach (var entry in entries)
            {
                entry.Entity.ModifiedDate = DateTime.UtcNow;
                entry.Entity.ModifiedBy = " Sample User 1";

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedBy = " Sample User";
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public DateTime GetEarliestTeamMatch(int teamId) => throw new NotImplementedException();
    }
}
