using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreApi.Entity.Migrations
{
    /// <inheritdoc />
    public partial class RenameCreatorUpdaterToCreateByUpdateBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdaterId",
                table: "Users",
                newName: "UpdateBy");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Users",
                newName: "CreateBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateBy",
                table: "Users",
                newName: "UpdaterId");

            migrationBuilder.RenameColumn(
                name: "CreateBy",
                table: "Users",
                newName: "CreatorId");
        }
    }
}
