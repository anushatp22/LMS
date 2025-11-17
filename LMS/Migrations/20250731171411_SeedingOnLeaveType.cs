using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class SeedingOnLeaveType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllocationFrequenancy",
                table: "leaveType");

            migrationBuilder.AlterColumn<string>(
                name: "GenderRestriction",
                table: "leaveType",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AllocationFrequency",
                table: "leaveType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "leaveType",
                columns: new[] { "Id", "AllocationFrequency", "DefaultAllocation", "GenderRestriction", "IsActive", "IsDeleted", "Type" },
                values: new object[,]
                {
                    { 1, "Yearly", 7, null, true, false, "Casual Leave" },
                    { 2, "Monthly", 12, null, true, false, "Sick Leave" },
                    { 3, "Yearly", 24, null, true, false, "Annual Leave" },
                    { 4, "Yearly", 5, null, true, false, "Compensatory Leave" },
                    { 5, "Yearly", 15, null, true, false, "Earned Leave" },
                    { 6, "Yearly", 0, null, true, false, "Leave Without Pay" },
                    { 7, "Yearly", 42, "Female", true, false, "Leave for miscarriage" },
                    { 8, "Yearly", 182, "Female", true, false, "Maternity Leave" },
                    { 9, "Monthly", 12, "Female", true, false, "Period Leave" },
                    { 10, "Yearly", 20, "Male", true, false, "Paternity Leave" },
                    { 11, "Yearly", 10, null, true, false, "Marriage Leave" },
                    { 12, "Yearly", 5, null, true, false, "Bereavement Leave" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "leaveType",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "leaveType",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "leaveType",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "leaveType",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "leaveType",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "leaveType",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "leaveType",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "leaveType",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "leaveType",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "leaveType",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "leaveType",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "leaveType",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DropColumn(
                name: "AllocationFrequency",
                table: "leaveType");

            migrationBuilder.AlterColumn<string>(
                name: "GenderRestriction",
                table: "leaveType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AllocationFrequenancy",
                table: "leaveType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
