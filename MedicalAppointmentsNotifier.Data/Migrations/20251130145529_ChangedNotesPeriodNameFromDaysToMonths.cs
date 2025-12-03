using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointmentsNotifier.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedNotesPeriodNameFromDaysToMonths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DaysPeriod",
                table: "Notes",
                newName: "MonthsPeriod");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MonthsPeriod",
                table: "Notes",
                newName: "DaysPeriod");
        }
    }
}
