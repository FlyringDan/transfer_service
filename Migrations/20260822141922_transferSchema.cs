using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace transfer_service.Migrations
{
    /// <inheritdoc />
    public partial class transferSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "giud");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "giud",
                table: "Users",
                newName: "Name");
        }
    }
}
