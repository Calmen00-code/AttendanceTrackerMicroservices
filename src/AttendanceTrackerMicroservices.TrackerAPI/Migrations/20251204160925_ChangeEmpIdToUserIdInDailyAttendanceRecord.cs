using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceTrackerMicroservices.TrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEmpIdToUserIdInDailyAttendanceRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "DailyAttendanceRecords",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "DailyAttendanceRecords",
                newName: "EmployeeId");
        }
    }
}
