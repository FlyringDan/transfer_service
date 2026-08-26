using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace transfer_service.Migrations
{
    /// <inheritdoc />
    public partial class addtransfervalidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_User_Balance_NonNegative",
                table: "Users",
                sql: "\"balance\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Transfer_Amount_Positive",
                table: "Transfers",
                sql: "\"Amount\" > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_User_Balance_NonNegative",
                table: "Users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Transfer_Amount_Positive",
                table: "Transfers");
        }
    }
}
