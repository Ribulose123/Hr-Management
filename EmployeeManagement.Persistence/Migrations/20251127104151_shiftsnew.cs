using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class shiftsnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EarlyExit",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "IsAbsent",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "IsLate",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "LateMinutes",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "OvertimeHours",
                table: "Shifts");

            migrationBuilder.RenameColumn(
                name: "isOvernight",
                table: "Shifts",
                newName: "IsOvernight");

            migrationBuilder.RenameColumn(
                name: "BreakTime",
                table: "Shifts",
                newName: "BreakMinutes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsOvernight",
                table: "Shifts",
                newName: "isOvernight");

            migrationBuilder.RenameColumn(
                name: "BreakMinutes",
                table: "Shifts",
                newName: "BreakTime");

            migrationBuilder.AddColumn<bool>(
                name: "EarlyExit",
                table: "Shifts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAbsent",
                table: "Shifts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLate",
                table: "Shifts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "LateMinutes",
                table: "Shifts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "OvertimeHours",
                table: "Shifts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
