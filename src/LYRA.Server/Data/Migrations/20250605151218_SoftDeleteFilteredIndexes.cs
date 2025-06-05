using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LYRA.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteFilteredIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing unfiltered indexes
            migrationBuilder.DropIndex(
                name: "IX_Companies_SystemName",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_TrustedTouchpoints_SystemName",
                table: "TrustedTouchpoints");

            // Create new filtered unique indexes (PostgreSQL syntax)
            migrationBuilder.Sql(
                @"CREATE UNIQUE INDEX IX_Companies_SystemName_Active
                  ON ""Companies"" (""SystemName"")
                  WHERE ""IsDeleted"" = false;");

            migrationBuilder.Sql(
                @"CREATE UNIQUE INDEX IX_TrustedTouchpoints_SystemName_Active
                  ON ""TrustedTouchpoints"" (""SystemName"")
                  WHERE ""IsDeleted"" = false;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop filtered indexes
            migrationBuilder.Sql(
                @"DROP INDEX IF EXISTS IX_Companies_SystemName_Active;");

            migrationBuilder.Sql(
                @"DROP INDEX IF EXISTS IX_TrustedTouchpoints_SystemName_Active;");

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
        }
    }
}
