using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseExecution.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "Name", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("f0b57672-6ec3-45d6-bf6b-5f6d07ba2ac1"), "A-01", new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc), "Inbound Buffer A-01", new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a6de81a2-827a-4ef8-ab80-69a304a4c613"), "A-02", new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc), "Inbound Buffer A-02", new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("4b250f3b-6537-4ca8-872f-fb702db9d712"), "B-01", new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc), "Storage Lane B-01", new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("77bfa0d1-517e-48ec-8846-5646041ebf66"), "B-02", new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc), "Storage Lane B-02", new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("97ccb9f8-525f-43b8-80d7-04727a2adfe4"), "P-01", new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc), "Packing Station P-01", new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.AddColumn<Guid>(
                name: "FromLocationId",
                table: "Jobs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ToLocationId",
                table: "Jobs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FromLocationId",
                table: "JobSteps",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ToLocationId",
                table: "JobSteps",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Jobs" j
                SET "FromLocationId" = l."Id"
                FROM "Locations" l
                WHERE j."FromLocation" = l."Code";
                """);

            migrationBuilder.Sql("""
                UPDATE "Jobs" j
                SET "ToLocationId" = l."Id"
                FROM "Locations" l
                WHERE j."ToLocation" = l."Code";
                """);

            migrationBuilder.Sql("""
                UPDATE "JobSteps" js
                SET "FromLocationId" = l."Id"
                FROM "Locations" l
                WHERE js."FromLocation" = l."Code";
                """);

            migrationBuilder.Sql("""
                UPDATE "JobSteps" js
                SET "ToLocationId" = l."Id"
                FROM "Locations" l
                WHERE js."ToLocation" = l."Code";
                """);

            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM "Jobs" WHERE "FromLocationId" IS NULL OR "ToLocationId" IS NULL) THEN
                        RAISE EXCEPTION 'Jobs contain location codes that do not exist in Locations seed data.';
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM "JobSteps" WHERE "FromLocationId" IS NULL OR "ToLocationId" IS NULL) THEN
                        RAISE EXCEPTION 'JobSteps contain location codes that do not exist in Locations seed data.';
                    END IF;
                END $$;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "ToLocationId",
                table: "Jobs",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FromLocationId",
                table: "Jobs",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ToLocationId",
                table: "JobSteps",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FromLocationId",
                table: "JobSteps",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "FromLocation",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ToLocation",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "FromLocation",
                table: "JobSteps");

            migrationBuilder.DropColumn(
                name: "ToLocation",
                table: "JobSteps");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_FromLocationId",
                table: "Jobs",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ToLocationId",
                table: "Jobs",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSteps_FromLocationId",
                table: "JobSteps",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSteps_ToLocationId",
                table: "JobSteps",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Code",
                table: "Locations",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Locations_FromLocationId",
                table: "Jobs",
                column: "FromLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Locations_ToLocationId",
                table: "Jobs",
                column: "ToLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSteps_Locations_FromLocationId",
                table: "JobSteps",
                column: "FromLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSteps_Locations_ToLocationId",
                table: "JobSteps",
                column: "ToLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromLocation",
                table: "Jobs",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToLocation",
                table: "Jobs",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FromLocation",
                table: "JobSteps",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToLocation",
                table: "JobSteps",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE "Jobs" j
                SET "FromLocation" = l."Code"
                FROM "Locations" l
                WHERE j."FromLocationId" = l."Id";
                """);

            migrationBuilder.Sql("""
                UPDATE "Jobs" j
                SET "ToLocation" = l."Code"
                FROM "Locations" l
                WHERE j."ToLocationId" = l."Id";
                """);

            migrationBuilder.Sql("""
                UPDATE "JobSteps" js
                SET "FromLocation" = l."Code"
                FROM "Locations" l
                WHERE js."FromLocationId" = l."Id";
                """);

            migrationBuilder.Sql("""
                UPDATE "JobSteps" js
                SET "ToLocation" = l."Code"
                FROM "Locations" l
                WHERE js."ToLocationId" = l."Id";
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Locations_FromLocationId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Locations_ToLocationId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSteps_Locations_FromLocationId",
                table: "JobSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSteps_Locations_ToLocationId",
                table: "JobSteps");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_FromLocationId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_ToLocationId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_JobSteps_FromLocationId",
                table: "JobSteps");

            migrationBuilder.DropIndex(
                name: "IX_JobSteps_ToLocationId",
                table: "JobSteps");

            migrationBuilder.DropColumn(
                name: "FromLocationId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ToLocationId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "FromLocationId",
                table: "JobSteps");

            migrationBuilder.DropColumn(
                name: "ToLocationId",
                table: "JobSteps");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
