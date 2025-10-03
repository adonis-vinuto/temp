using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.Master;

public partial class PrimeiraMigrationMaster : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "DataConfigs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                Module = table.Column<int>(type: "int", nullable: false),
                Organization = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 150, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                SqlHost = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 150, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                SqlPort = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 150, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                SqlUser = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 100, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                SqlPassword = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 100, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                SqlDatabase = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 100, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                BlobConnectionString = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 500, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                BlobContainerName = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 100, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_DataConfigs", x => x.Id)
        )
        .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Logs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                IdUser = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                NameUser = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                UserEmail = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                ChangedEntity = table.Column<string>(type: "varchar(400)", unicode: false, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                TypeProcess = table.Column<int>(type: "int", nullable: false),
                LastState = table.Column<string>(type: "TEXT", unicode: false, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                NewState = table.Column<string>(type: "TEXT", unicode: false, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Logs", x => x.Id)
        )
        .Annotation("MySql:CharSet", "utf8mb4");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "DataConfigs");
        migrationBuilder.DropTable(name: "Logs");
    }
}
