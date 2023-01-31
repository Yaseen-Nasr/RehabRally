using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RehabRally.Web.Data.Migrations
{
    public partial class Add_Fake_External_linkImages_For_Exercise_Module : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkImageUrl",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryImageUrl",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkImageUrl",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "SecondaryImageUrl",
                table: "Exercises");
        }
    }
}
