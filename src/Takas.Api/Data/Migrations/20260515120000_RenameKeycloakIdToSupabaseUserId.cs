using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Takas.Api.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260515120000_RenameKeycloakIdToSupabaseUserId")]
    public partial class RenameKeycloakIdToSupabaseUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KeycloakId",
                table: "Users",
                newName: "SupabaseUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_KeycloakId",
                table: "Users",
                newName: "IX_Users_SupabaseUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SupabaseUserId",
                table: "Users",
                newName: "KeycloakId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_SupabaseUserId",
                table: "Users",
                newName: "IX_Users_KeycloakId");
        }
    }
}
