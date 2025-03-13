using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.DAL.Presistance.Data.Migrations
{
    /// <inheritdoc />
    public partial class Uploadimage9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Img",
                table: "Employees",
                newName: "ImagePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Employees",
                newName: "Img");
        }
    }
}
