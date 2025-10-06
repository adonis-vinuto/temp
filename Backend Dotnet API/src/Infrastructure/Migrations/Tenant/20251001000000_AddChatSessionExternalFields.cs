using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.Tenant;

    /// <inheritdoc />
    public partial class AddChatSessionExternalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdSession",
                table: "ChatSessions",
                type: "varchar(400)",
                unicode: false,
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ChatSessions",
                type: "varchar(400)",
                unicode: false,
                maxLength: 400,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdSession",
                table: "ChatSessions");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ChatSessions");
        }
    }
