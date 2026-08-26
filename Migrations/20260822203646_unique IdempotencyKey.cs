using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace transfer_service.Migrations
{
    /// <inheritdoc />
    public partial class uniqueIdempotencyKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transfers_IdempotencyKey",
                table: "Transfers",
                column: "IdempotencyKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transfers_IdempotencyKey",
                table: "Transfers");
        }
    }
}
