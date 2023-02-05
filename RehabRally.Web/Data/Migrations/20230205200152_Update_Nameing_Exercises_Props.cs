using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RehabRally.Web.Data.Migrations
{
    public partial class Update_Nameing_Exercises_Props : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FourthImageUrl",
                table: "Exercises");

            migrationBuilder.RenameColumn(
                name: "ThirdImageUrl",
                table: "Exercises",
                newName: "ImageThirdUrl");

            migrationBuilder.RenameColumn(
                name: "SecondaryImageUrl",
                table: "Exercises",
                newName: "ImageSecondaryUrl");

            migrationBuilder.RenameColumn(
                name: "LinkImageUrl",
                table: "Exercises",
                newName: "ImageLinkUrl");

            migrationBuilder.RenameColumn(
                name: "ImagePublicId",
                table: "Exercises",
                newName: "ImageFourthUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageThirdUrl",
                table: "Exercises",
                newName: "ThirdImageUrl");

            migrationBuilder.RenameColumn(
                name: "ImageSecondaryUrl",
                table: "Exercises",
                newName: "SecondaryImageUrl");

            migrationBuilder.RenameColumn(
                name: "ImageLinkUrl",
                table: "Exercises",
                newName: "LinkImageUrl");

            migrationBuilder.RenameColumn(
                name: "ImageFourthUrl",
                table: "Exercises",
                newName: "ImagePublicId");

            migrationBuilder.AddColumn<string>(
                name: "FourthImageUrl",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
