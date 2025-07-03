using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LYRA.Server.Data.LyraCachedDb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CachedAccessPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CallerSystemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TargetSystemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Context = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CallerSecret = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignatureType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AllowedSourceIp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CallerCompanySystemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TargetCompanySystemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CachedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CachedAccessPolicies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CachedAccessPolicy_Key",
                table: "CachedAccessPolicies",
                columns: new[] { "CallerSystemName", "TargetSystemName", "Context" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CachedAccessPolicies");
        }
    }
}
