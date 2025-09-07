using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreApi.Entity.Migrations
{
    /// <inheritdoc />
    public partial class RenameCreaterIdToCreatorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreaterId",
                table: "Users",
                newName: "CreatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Users",
                newName: "CreaterId");
        }
    }
}
