using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RehabRally.EF.Migrations
{
    public partial class rename_Precaution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Conclusion",
                table: "PatientConclusions",
                newName: "Precaution");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Precaution",
                table: "PatientConclusions",
                newName: "Conclusion");
        }
    }
}
