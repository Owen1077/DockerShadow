using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DockerShadow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "NEXTGEN");

            migrationBuilder.CreateTable(
                name: "CPOT_ROLE",
                schema: "NEXTGEN",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NORMALIZED_NAME = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    CONCURRENCY_STAMP = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPOT_ROLE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CPOT_USER",
                schema: "NEXTGEN",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    IS_LOGGED_IN = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    LAST_LOGIN_TIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NORMALIZED_USER_NAME = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NORMALIZED_EMAIL = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    EMAIL_CONFIRMED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PASSWORD_HASH = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SECURITY_STAMP = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CONCURRENCY_STAMP = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PHONE_NUMBER = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PHONE_NUMBER_CONFIRMED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TWO_FACTOR_ENABLED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    LOCKOUT_END = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    LOCKOUT_ENABLED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ACCESS_FAILED_COUNT = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPOT_USER", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CPOT_ROLE_CLAIMS",
                schema: "NEXTGEN",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ROLE_ID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    CLAIM_TYPE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CLAIM_VALUE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPOT_ROLE_CLAIMS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CPOT_ROLE_CLAIMS_CPOT_ROLE~",
                        column: x => x.ROLE_ID,
                        principalSchema: "NEXTGEN",
                        principalTable: "CPOT_ROLE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CPOT_USER_CLAIMS",
                schema: "NEXTGEN",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USER_ID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    CLAIM_TYPE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CLAIM_VALUE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPOT_USER_CLAIMS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CPOT_USER_CLAIMS_CPOT_USER~",
                        column: x => x.USER_ID,
                        principalSchema: "NEXTGEN",
                        principalTable: "CPOT_USER",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CPOT_USER_LOGINS",
                schema: "NEXTGEN",
                columns: table => new
                {
                    LOGIN_PROVIDER = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    PROVIDER_KEY = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    PROVIDER_DISPLAY_NAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    USER_ID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPOT_USER_LOGINS", x => new { x.LOGIN_PROVIDER, x.PROVIDER_KEY });
                    table.ForeignKey(
                        name: "FK_CPOT_USER_LOGINS_CPOT_USER~",
                        column: x => x.USER_ID,
                        principalSchema: "NEXTGEN",
                        principalTable: "CPOT_USER",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CPOT_USER_ROLES",
                schema: "NEXTGEN",
                columns: table => new
                {
                    USER_ID = table.Column<string>(type: "NVARCHAR2(450)", maxLength: 450, nullable: false),
                    ROLE_ID = table.Column<string>(type: "NVARCHAR2(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPOT_USER_ROLES", x => new { x.USER_ID, x.ROLE_ID });
                    table.ForeignKey(
                        name: "FK_CPOT_USER_ROLES_CPOT_ROLE_~",
                        column: x => x.ROLE_ID,
                        principalSchema: "NEXTGEN",
                        principalTable: "CPOT_ROLE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CPOT_USER_ROLES_CPOT_USER_~",
                        column: x => x.USER_ID,
                        principalSchema: "NEXTGEN",
                        principalTable: "CPOT_USER",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CPOT_USER_TOKENS",
                schema: "NEXTGEN",
                columns: table => new
                {
                    USER_ID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    LOGIN_PROVIDER = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    VALUE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPOT_USER_TOKENS", x => new { x.USER_ID, x.LOGIN_PROVIDER, x.NAME });
                    table.ForeignKey(
                        name: "FK_CPOT_USER_TOKENS_CPOT_USER~",
                        column: x => x.USER_ID,
                        principalSchema: "NEXTGEN",
                        principalTable: "CPOT_USER",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "NEXTGEN",
                table: "CPOT_ROLE",
                columns: new[] { "ID", "CONCURRENCY_STAMP", "NAME", "NORMALIZED_NAME" },
                values: new object[,]
                {
                    { "510057bf-a91a-4398-83e7-58a558ae5edd", "71f781f7-e957-469b-96df-9f2035147a23", "Administrator", "ADMINISTRATOR" },
                    { "76cdb59e-48da-4651-b300-a20e9c08a750", "71f781f7-e957-469b-96df-9f2035147a56", "Log", "LOG" }
                });

            migrationBuilder.InsertData(
                schema: "NEXTGEN",
                table: "CPOT_USER",
                columns: new[] { "ID", "ACCESS_FAILED_COUNT", "CONCURRENCY_STAMP", "CREATED_AT", "EMAIL", "EMAIL_CONFIRMED", "IS_LOGGED_IN", "LAST_LOGIN_TIME", "LOCKOUT_ENABLED", "LOCKOUT_END", "NAME", "NORMALIZED_EMAIL", "NORMALIZED_USER_NAME", "PASSWORD_HASH", "PHONE_NUMBER", "PHONE_NUMBER_CONFIRMED", "SECURITY_STAMP", "STATUS", "TWO_FACTOR_ENABLED", "UPDATED_AT", "USER_NAME" },
                values: new object[,]
                {
                    { "7cc5cd62-6240-44e5-b44f-bff0ae73342", 0, "71f781f7-e957-469b-96df-9f2035147e45", new DateTime(2023, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Oluwatosin.Shada@ACCESSBANKPLC.com", true, false, new DateTime(2023, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "Oluwatosin Shada", "OLUWATOSIN.SHADA@ACCESSBANKPLC.COM", "SHADAO", "AQAAAAEAACcQAAAAEBLjouNqaeiVWbN0TbXUS3+ChW3d7aQIk/BQEkWBxlrdRRngp14b0BIH0Rp65qD6mA==", null, true, "71f781f7-e957-469b-96df-9f2035147e93", "Active", false, new DateTime(2023, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "shadao" },
                    { "9a6a928b-0e11-4d5d-8a29-b8f04445e72", 0, "71f781f7-e957-469b-96df-9f2035147e98", new DateTime(2023, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thelma.Ohue@ACCESSBANKPLC.com", true, false, new DateTime(2023, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "Thelma Ohue", "THELMA.OHUE@ACCESSBANKPLC.COM", "OHUET", "AQAAAAEAACcQAAAAEBLjouNqaeiVWbN0TbXUS3+ChW3d7aQIk/BQEkWBxlrdRRngp14b0BIH0Rp65qD6mA==", null, true, "71f781f7-e957-469b-96df-9f2035147e37", "Active", false, new DateTime(2023, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "ohuet" }
                });

            migrationBuilder.InsertData(
                schema: "NEXTGEN",
                table: "CPOT_USER_ROLES",
                columns: new[] { "ROLE_ID", "USER_ID" },
                values: new object[,]
                {
                    { "510057bf-a91a-4398-83e7-58a558ae5edd", "7cc5cd62-6240-44e5-b44f-bff0ae73342" },
                    { "76cdb59e-48da-4651-b300-a20e9c08a750", "9a6a928b-0e11-4d5d-8a29-b8f04445e72" }
                });

            migrationBuilder.CreateIndex(
                name: "CPOT_RoleNameIndex",
                schema: "NEXTGEN",
                table: "CPOT_ROLE",
                column: "NORMALIZED_NAME",
                unique: true,
                filter: "\"NORMALIZED_NAME\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CPOT_ROLE_CLAIMS_ROLE_ID",
                schema: "NEXTGEN",
                table: "CPOT_ROLE_CLAIMS",
                column: "ROLE_ID");

            migrationBuilder.CreateIndex(
                name: "CPOT_EmailIndex",
                schema: "NEXTGEN",
                table: "CPOT_USER",
                column: "NORMALIZED_EMAIL");

            migrationBuilder.CreateIndex(
                name: "CPOT_UserNameIndex",
                schema: "NEXTGEN",
                table: "CPOT_USER",
                column: "NORMALIZED_USER_NAME",
                unique: true,
                filter: "\"NORMALIZED_USER_NAME\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CPOT_USER_CLAIMS_USER_ID",
                schema: "NEXTGEN",
                table: "CPOT_USER_CLAIMS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CPOT_USER_LOGINS_USER_ID",
                schema: "NEXTGEN",
                table: "CPOT_USER_LOGINS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CPOT_USER_ROLES_ROLE_ID",
                schema: "NEXTGEN",
                table: "CPOT_USER_ROLES",
                column: "ROLE_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CPOT_ROLE_CLAIMS",
                schema: "NEXTGEN");

            migrationBuilder.DropTable(
                name: "CPOT_USER_CLAIMS",
                schema: "NEXTGEN");

            migrationBuilder.DropTable(
                name: "CPOT_USER_LOGINS",
                schema: "NEXTGEN");

            migrationBuilder.DropTable(
                name: "CPOT_USER_ROLES",
                schema: "NEXTGEN");

            migrationBuilder.DropTable(
                name: "CPOT_USER_TOKENS",
                schema: "NEXTGEN");

            migrationBuilder.DropTable(
                name: "CPOT_ROLE",
                schema: "NEXTGEN");

            migrationBuilder.DropTable(
                name: "CPOT_USER",
                schema: "NEXTGEN");
        }
    }
}
