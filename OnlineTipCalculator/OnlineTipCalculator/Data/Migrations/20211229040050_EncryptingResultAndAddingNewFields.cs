using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineTipCalculator.Data.Migrations
{
    public partial class EncryptingResultAndAddingNewFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ResultAmount",
                table: "Calculations",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Calculations",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Calculations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Calculations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Calculations");

            migrationBuilder.AlterColumn<double>(
                name: "ResultAmount",
                table: "Calculations",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
