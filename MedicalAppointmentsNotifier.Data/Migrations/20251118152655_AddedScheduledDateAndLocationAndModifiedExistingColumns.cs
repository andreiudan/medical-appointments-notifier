using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointmentsNotifier.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedScheduledDateAndLocationAndModifiedExistingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NextDate",
                table: "Appointments",
                newName: "ScheduledLocation");

            migrationBuilder.RenameColumn(
                name: "LatestDate",
                table: "Appointments",
                newName: "IssuedOn");

            migrationBuilder.RenameColumn(
                name: "IntervalDays",
                table: "Appointments",
                newName: "MonthsInterval");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ScheduledOn",
                table: "Appointments",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledOn",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "ScheduledLocation",
                table: "Appointments",
                newName: "NextDate");

            migrationBuilder.RenameColumn(
                name: "MonthsInterval",
                table: "Appointments",
                newName: "IntervalDays");

            migrationBuilder.RenameColumn(
                name: "IssuedOn",
                table: "Appointments",
                newName: "LatestDate");
        }
    }
}
