using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseExecution.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddJobNumberCounters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobNumberCounters",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    LastValue = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobNumberCounters", x => x.Date);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobNumberCounters");
        }
    }
}
