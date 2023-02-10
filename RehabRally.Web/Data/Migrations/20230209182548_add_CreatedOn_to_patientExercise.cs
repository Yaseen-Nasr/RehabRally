using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RehabRally.Web.Data.Migrations
{
    public partial class add_CreatedOn_to_patientExercise : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "PatientExercises",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "PatientExercises");
        }
    }
}
