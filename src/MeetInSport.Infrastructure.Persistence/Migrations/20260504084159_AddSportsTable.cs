using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetInSport.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSportsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sport",
                table: "Coaches");

            migrationBuilder.AddColumn<Guid>(
                name: "SportId",
                table: "Coaches",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Sports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sports", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Sports",
                columns: new[] { "Id", "Name", "CreatedAt", "UpdatedAt", "IsDeleted" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), "Uncategorized", DateTime.UtcNow, DateTime.UtcNow, false }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_SportId",
                table: "Coaches",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_Sports_Name",
                table: "Sports",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Coaches_Sports_SportId",
                table: "Coaches",
                column: "SportId",
                principalTable: "Sports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coaches_Sports_SportId",
                table: "Coaches");

            migrationBuilder.DropTable(
                name: "Sports");

            migrationBuilder.DropIndex(
                name: "IX_Coaches_SportId",
                table: "Coaches");

            migrationBuilder.DropColumn(
                name: "SportId",
                table: "Coaches");

            migrationBuilder.AddColumn<string>(
                name: "Sport",
                table: "Coaches",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
