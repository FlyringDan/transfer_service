using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace transfer_service.Migrations
{
    /// <inheritdoc />
    public partial class fixtransfertablee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "idempotencyKey",
                table: "Transfers",
                newName: "IdempotencyKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdempotencyKey",
                table: "Transfers",
                newName: "idempotencyKey");
        }
    }
}
