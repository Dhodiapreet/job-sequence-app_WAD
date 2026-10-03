using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSequenceApp.Migrations
{
    /// <inheritdoc />
    public partial class Phase4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Biography",
                table: "WorkerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Experience",
                table: "WorkerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Pricing",
                table: "WorkerProfiles",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Profession",
                table: "WorkerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceArea",
                table: "WorkerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "WorkerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedEndTime",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedStartTime",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Service",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "WorkerProfileId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_WorkerProfileId",
                table: "Bookings",
                column: "WorkerProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_WorkerProfiles_WorkerProfileId",
                table: "Bookings",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_WorkerProfiles_WorkerProfileId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_WorkerProfileId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Biography",
                table: "WorkerProfiles");

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "WorkerProfiles");

            migrationBuilder.DropColumn(
                name: "Pricing",
                table: "WorkerProfiles");

            migrationBuilder.DropColumn(
                name: "Profession",
                table: "WorkerProfiles");

            migrationBuilder.DropColumn(
                name: "ServiceArea",
                table: "WorkerProfiles");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "WorkerProfiles");

            migrationBuilder.DropColumn(
                name: "ExpectedEndTime",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ExpectedStartTime",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Service",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "WorkerProfileId",
                table: "Bookings");
        }
    }
}
