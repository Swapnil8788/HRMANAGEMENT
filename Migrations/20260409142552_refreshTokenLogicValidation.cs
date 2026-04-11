using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRManagement.Migrations
{
    /// <inheritdoc />
    public partial class refreshTokenLogicValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isRevoked",
                table: "RefreshTokens",
                newName: "IsRevoked");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRevoked",
                table: "RefreshTokens",
                newName: "isRevoked");
        }
    }
}
