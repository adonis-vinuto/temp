using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.Tenant;

/// <inheritdoc />
public partial class AddFileGeneratedName : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "GeneratedName",
            table: "File",
            type: "varchar(400)",
            unicode: false,
            maxLength: 1000,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "GeneratedName",
            table: "File");
    }
}
