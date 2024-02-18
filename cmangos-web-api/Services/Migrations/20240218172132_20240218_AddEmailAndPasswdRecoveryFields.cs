using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Migrations
{
    /// <inheritdoc />
    public partial class _20240218_AddEmailAndPasswdRecoveryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Created",
                table: "account_ext",
                newName: "PendingEmailTokenSent");

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordRecoverySent",
                table: "account_ext",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordRecoveryToken",
                table: "account_ext",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PendingEmailToken",
                table: "account_ext",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordRecoverySent",
                table: "account_ext");

            migrationBuilder.DropColumn(
                name: "PasswordRecoveryToken",
                table: "account_ext");

            migrationBuilder.DropColumn(
                name: "PendingEmailToken",
                table: "account_ext");

            migrationBuilder.RenameColumn(
                name: "PendingEmailTokenSent",
                table: "account_ext",
                newName: "Created");
        }
    }
}
