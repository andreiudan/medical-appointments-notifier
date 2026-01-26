using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointmentsNotifier.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedNoteTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Until",
                table: "Notes",
                newName: "Title");

            migrationBuilder.AddColumn<int>(
                name: "DaysPeriod",
                table: "Notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysPeriod",
                table: "Notes");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Notes",
                newName: "Until");
        }
    }
}
