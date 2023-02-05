using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RehabRally.Web.Data.Migrations
{
    public partial class addIsDoneToPatientExerciseTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "PatientExercises",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDone",
                table: "PatientExercises");
        }
    }
}
