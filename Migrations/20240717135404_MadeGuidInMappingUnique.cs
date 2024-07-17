using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WireMock.Migrations
{
    /// <inheritdoc />
    public partial class MadeGuidInMappingUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WireMockServerMapping_Guid",
                table: "WireMockServerMapping",
                column: "Guid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WireMockServerMapping_Guid",
                table: "WireMockServerMapping");
        }
    }
}
