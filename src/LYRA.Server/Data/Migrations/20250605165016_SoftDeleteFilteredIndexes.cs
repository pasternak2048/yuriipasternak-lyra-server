using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LYRA.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteFilteredIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing unfiltered indexes using raw SQL to avoid "index does not exist" error
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Companies_SystemName"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_TrustedTouchpoints_SystemName"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_AccessPolicy_Key"";");

            // Create filtered unique index for active companies (PostgreSQL syntax)
            migrationBuilder.Sql(
                @"CREATE UNIQUE INDEX IX_Companies_SystemName_Active
              ON ""Companies"" (""SystemName"")
              WHERE ""IsDeleted"" = false AND ""IsActive"" = true;");

            // Create filtered unique index for active touchpoints (PostgreSQL syntax)
            migrationBuilder.Sql(
                @"CREATE UNIQUE INDEX IX_TrustedTouchpoints_SystemName_Active
              ON ""TrustedTouchpoints"" (""SystemName"")
              WHERE ""IsDeleted"" = false AND ""IsActive"" = true;");

            // Create composite unique index for access policies (unfiltered)
            migrationBuilder.CreateIndex(
                name: "IX_AccessPolicy_Key",
                table: "AccessPolicies",
                columns: new[] { "CallerSystemName", "TargetSystemName", "Context", "Operation" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop filtered indexes
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS IX_Companies_SystemName_Active;");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS IX_TrustedTouchpoints_SystemName_Active;");

            // Drop composite access policy index
            migrationBuilder.DropIndex(
                name: "IX_AccessPolicy_Key",
                table: "AccessPolicies");

            // Recreate original unfiltered indexes
            migrationBuilder.CreateIndex(
                name: "IX_Companies_SystemName",
                table: "Companies",
                column: "SystemName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrustedTouchpoints_SystemName",
                table: "TrustedTouchpoints",
                column: "SystemName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccessPolicy_Key",
                table: "AccessPolicies",
                columns: new[] { "CallerSystemName", "TargetSystemName", "Context", "Operation" },
                unique: true);
        }
    }
}
