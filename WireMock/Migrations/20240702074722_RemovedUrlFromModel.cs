using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WireMock.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUrlFromModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Urls",
                table: "WireMockServerModel",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "WireMockServerModel",
                newName: "Urls");
        }
    }
}
