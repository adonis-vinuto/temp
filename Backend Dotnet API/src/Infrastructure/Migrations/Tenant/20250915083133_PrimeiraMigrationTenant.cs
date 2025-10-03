using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.Tenant;

    /// <inheritdoc />
    public partial class PrimeiraMigrationTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Organization = table.Column<string>(type: "varchar(400)", unicode: false, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Module = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                    constraints: table => table.PrimaryKey("PK_Agents", x => x.Id)
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Module = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "LONGTEXT", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalPages = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Summary = table.Column<string>(type: "LONGTEXT", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Answer = table.Column<string>(type: "LONGTEXT", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompletionTokens = table.Column<int>(type: "int", nullable: true),
                    PromptTokens = table.Column<int>(type: "int", nullable: true),
                    TotalTokens = table.Column<int>(type: "int", nullable: true),
                    CompletionTime = table.Column<int>(type: "int", nullable: true),
                    PromptTime = table.Column<int>(type: "int", nullable: true),
                    QueueTime = table.Column<int>(type: "int", nullable: true),
                    TotalTime = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                    constraints: table => table.PrimaryKey("PK_File", x => x.Id)
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Knowledge",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Module = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Origin = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                    constraints: table => table.PrimaryKey("PK_Knowledge", x => x.Id)
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

            migrationBuilder.CreateTable(
                name: "SeniorErpConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Username = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WsdlUrl = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_SeniorErpConfig", x => x.Id)
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SeniorHcmConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Username = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WsdlUrl = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                    constraints: table => table.PrimaryKey("PK_SeniorHcmConfig", x => x.Id)
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ChatSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IdAgent = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TotalInteractions = table.Column<int>(type: "int", nullable: false),
                    IdUser = table.Column<string>(type: "varchar(400)", unicode: false, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastSendDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatSessions_Agents_IdAgent",
                        column: x => x.IdAgent,
                        principalTable: "Agents",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TwilioConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IdAgent = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AccountSid = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthToken = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WebhookUrl = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwilioConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TwilioConfig_Agents_IdAgent",
                        column: x => x.IdAgent,
                        principalTable: "Agents",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AgentFile",
                columns: table => new
                {
                    AgentsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FilesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentFile", x => new { x.AgentsId, x.FilesId });
                    table.ForeignKey(
                        name: "FK_AgentFile_Agents_AgentsId",
                        column: x => x.AgentsId,
                        principalTable: "Agents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AgentFile_File_FilesId",
                        column: x => x.FilesId,
                        principalTable: "File",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Page",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IdFile = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PageNumber = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "varchar(400)", unicode: false, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResumePage = table.Column<string>(type: "varchar(400)", unicode: false, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Page", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Page_File_IdFile",
                        column: x => x.IdFile,
                        principalTable: "File",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AgentKnowledge",
                columns: table => new
                {
                    AgentsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    KnowledgesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentKnowledge", x => new { x.AgentsId, x.KnowledgesId });
                    table.ForeignKey(
                        name: "FK_AgentKnowledge_Agents_AgentsId",
                        column: x => x.AgentsId,
                        principalTable: "Agents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AgentKnowledge_Knowledge_KnowledgesId",
                        column: x => x.KnowledgesId,
                        principalTable: "Knowledge",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(400)", unicode: false, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdKnowledge = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CompanyName = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FullName = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdmissionDate = table.Column<DateTime>(type: "date", nullable: true),
                    TerminationDate = table.Column<DateTime>(type: "date", nullable: true),
                    StatusDescription = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: true),
                    CostCneterName = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Salary = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    ComplementarySalary = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    SalaryEffectiveDate = table.Column<DateTime>(type: "date", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    StreetAddress = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AddressNumber = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CityName = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Race = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PostalCode = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyCodSeniorNumEmp = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeeCodSeniorNumCad = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CollaboratorTypeCodeSeniorTipeCol = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StatusCodSenior = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CostCenterCodSeniorCodCcu = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Knowledge_IdKnowledge",
                        column: x => x.IdKnowledge,
                        principalTable: "Knowledge",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AgentSeniorErpConfig",
                columns: table => new
                {
                    AgentsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SeniorErpConfigsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentSeniorErpConfig", x => new { x.AgentsId, x.SeniorErpConfigsId });
                    table.ForeignKey(
                        name: "FK_AgentSeniorErpConfig_Agents_AgentsId",
                        column: x => x.AgentsId,
                        principalTable: "Agents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AgentSeniorErpConfig_SeniorErpConfig_SeniorErpConfigsId",
                        column: x => x.SeniorErpConfigsId,
                        principalTable: "SeniorErpConfig",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AgentSeniorHcmConfig",
                columns: table => new
                {
                    AgentsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SeniorHcmConfigsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentSeniorHcmConfig", x => new { x.AgentsId, x.SeniorHcmConfigsId });
                    table.ForeignKey(
                        name: "FK_AgentSeniorHcmConfig_Agents_AgentsId",
                        column: x => x.AgentsId,
                        principalTable: "Agents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AgentSeniorHcmConfig_SeniorHcmConfig_SeniorHcmConfigsId",
                        column: x => x.SeniorHcmConfigsId,
                        principalTable: "SeniorHcmConfig",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ChatHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IdChatSession = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Content = table.Column<string>(type: "text", unicode: false, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdUser = table.Column<string>(type: "varchar(400)", unicode: false, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<int>(type: "int", nullable: false),
                    TotalTokens = table.Column<int>(type: "int", nullable: false),
                    TotalTime = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatHistory_ChatSessions_IdChatSession",
                        column: x => x.IdChatSession,
                        principalTable: "ChatSessions",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Payroll",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IdEmployee = table.Column<string>(type: "varchar(400)", unicode: false, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PayrollPeriodCod = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EventName = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EventAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EventTypeName = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReferenceDate = table.Column<DateTime>(type: "date", nullable: true),
                    CalculationTypeName = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeeCodSeniorNumCad = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CollaboratorTypeCodeSeniorTipCol = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyCodSeniorNumEmp = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PayrollPeriodCodSeniorCodCal = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EventTypeCodSeniorTipEve = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EventCodSeniorCodenv = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CalculationTypeCodSeniorTipCal = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payroll", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payroll_Employee_IdEmployee",
                        column: x => x.IdEmployee,
                        principalTable: "Employee",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SalaryHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IdEmployee = table.Column<string>(type: "varchar(400)", unicode: false, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ChangeDate = table.Column<DateTime>(type: "date", nullable: true),
                    NewSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MotiveName = table.Column<string>(type: "varchar(400)", unicode: false, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeeCodSeniorNumCad = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyCodSeniorNumEmp = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyCodSeniorCodFil = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MotiveCodSeniorCodMot = table.Column<string>(type: "varchar(400)", unicode: false, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryHistory_Employee_IdEmployee",
                        column: x => x.IdEmployee,
                        principalTable: "Employee",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AgentFile_FilesId",
                table: "AgentFile",
                column: "FilesId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentKnowledge_KnowledgesId",
                table: "AgentKnowledge",
                column: "KnowledgesId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentSeniorErpConfig_SeniorErpConfigsId",
                table: "AgentSeniorErpConfig",
                column: "SeniorErpConfigsId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentSeniorHcmConfig_SeniorHcmConfigsId",
                table: "AgentSeniorHcmConfig",
                column: "SeniorHcmConfigsId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatHistory_IdChatSession",
                table: "ChatHistory",
                column: "IdChatSession");

            migrationBuilder.CreateIndex(
                name: "IX_ChatSessions_IdAgent",
                table: "ChatSessions",
                column: "IdAgent");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_IdKnowledge",
                table: "Employee",
                column: "IdKnowledge");

            migrationBuilder.CreateIndex(
                name: "IX_Page_IdFile",
                table: "Page",
                column: "IdFile");

            migrationBuilder.CreateIndex(
                name: "IX_Payroll_IdEmployee",
                table: "Payroll",
                column: "IdEmployee");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryHistory_IdEmployee",
                table: "SalaryHistory",
                column: "IdEmployee");

            migrationBuilder.CreateIndex(
                name: "IX_TwilioConfig_IdAgent",
                table: "TwilioConfig",
                column: "IdAgent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentFile");

            migrationBuilder.DropTable(
                name: "AgentKnowledge");

            migrationBuilder.DropTable(
                name: "AgentSeniorErpConfig");

            migrationBuilder.DropTable(
                name: "AgentSeniorHcmConfig");

            migrationBuilder.DropTable(
                name: "ChatHistory");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Page");

            migrationBuilder.DropTable(
                name: "Payroll");

            migrationBuilder.DropTable(
                name: "SalaryHistory");

            migrationBuilder.DropTable(
                name: "TwilioConfig");

            migrationBuilder.DropTable(
                name: "SeniorErpConfig");

            migrationBuilder.DropTable(
                name: "SeniorHcmConfig");

            migrationBuilder.DropTable(
                name: "ChatSessions");

            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Agents");

            migrationBuilder.DropTable(
                name: "Knowledge");
        }
    }
