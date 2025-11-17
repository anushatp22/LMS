using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class allocationinLeave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxDays",
                table: "leaveType",
                newName: "DefaultAllocation");

            migrationBuilder.AddColumn<string>(
                name: "AllocationFrequenancy",
                table: "leaveType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAllocatedDate",
                table: "leaveBalance",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllocationFrequenancy",
                table: "leaveType");

            migrationBuilder.DropColumn(
                name: "LastAllocatedDate",
                table: "leaveBalance");

            migrationBuilder.RenameColumn(
                name: "DefaultAllocation",
                table: "leaveType",
                newName: "MaxDays");
        }
    }
}
