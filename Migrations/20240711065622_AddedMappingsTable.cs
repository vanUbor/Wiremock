using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WireMock.Migrations
{
    /// <inheritdoc />
    public partial class AddedMappingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WireMockServerMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Raw = table.Column<string>(type: "TEXT", nullable: false),
                    WireMockServerModelId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WireMockServerMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WireMockServerMapping_WireMockServerModel_WireMockServerModelId",
                        column: x => x.WireMockServerModelId,
                        principalTable: "WireMockServerModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WireMockServerMapping_WireMockServerModelId",
                table: "WireMockServerMapping",
                column: "WireMockServerModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WireMockServerMapping");
        }
    }
}
