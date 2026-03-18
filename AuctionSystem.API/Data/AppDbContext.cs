using AuctionSystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Auction> Auctions => Set<Auction>();
    public DbSet<Bid> Bids => Set<Bid>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auction>()
            .Property(a => a.RowVersion)
            .IsRowVersion();

        modelBuilder.Entity<Auction>()
            .Property(a => a.CurrentPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Auction>()
            .Property(a => a.StartingPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Bid>()
            .Property(b => b.Amount)
            .HasColumnType("decimal(18,2)");

        // Seed Data
        modelBuilder.Entity<Auction>().HasData(
            new Auction
            {
                Id = 1,
                Title = "שעון וינטאג' נדיר",
                Description = "שעון יד מזהב משנת 1950",
                StartingPrice = 500,
                CurrentPrice = 500,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
                Status = AuctionStatus.Active,
                RowVersion = Array.Empty<byte>()
            },
            new Auction
            {
                Id = 2,
                Title = "ציור מקורי",
                Description = "ציור שמן על קנבס",
                StartingPrice = 1000,
                CurrentPrice = 1000,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                Status = AuctionStatus.Active,
                RowVersion = Array.Empty<byte>()
            }
        );
    }
}