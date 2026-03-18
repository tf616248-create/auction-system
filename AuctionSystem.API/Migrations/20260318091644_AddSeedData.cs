using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2026, 3, 18, 11, 16, 44, 203, DateTimeKind.Utc).AddTicks(4889), new DateTime(2026, 3, 18, 9, 16, 44, 203, DateTimeKind.Utc).AddTicks(4888) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2026, 3, 18, 10, 16, 44, 203, DateTimeKind.Utc).AddTicks(4895), new DateTime(2026, 3, 18, 9, 16, 44, 203, DateTimeKind.Utc).AddTicks(4895) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2026, 3, 18, 11, 6, 50, 564, DateTimeKind.Utc).AddTicks(628), new DateTime(2026, 3, 18, 9, 6, 50, 564, DateTimeKind.Utc).AddTicks(627) });

            migrationBuilder.UpdateData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2026, 3, 18, 10, 6, 50, 564, DateTimeKind.Utc).AddTicks(635), new DateTime(2026, 3, 18, 9, 6, 50, 564, DateTimeKind.Utc).AddTicks(635) });
        }
    }
}
