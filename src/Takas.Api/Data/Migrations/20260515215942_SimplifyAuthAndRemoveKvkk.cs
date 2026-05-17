using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Takas.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyAuthAndRemoveKvkk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_SupabaseUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "KvkkAccepted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "KvkkAcceptedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "KvkkVersion",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SupabaseUserId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "KvkkAccepted",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "KvkkAcceptedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KvkkVersion",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SupabaseUserId",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SupabaseUserId",
                table: "Users",
                column: "SupabaseUserId",
                unique: true);
        }
    }
}
