using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WireMock.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProxyUrl",
                table: "WireMockServerModel",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "SaveMapping",
                table: "WireMockServerModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SaveMappingForStatusCodePattern",
                table: "WireMockServerModel",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "SaveMappingToFile",
                table: "WireMockServerModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StartAdminInterface",
                table: "WireMockServerModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Urls",
                table: "WireMockServerModel",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProxyUrl",
                table: "WireMockServerModel");

            migrationBuilder.DropColumn(
                name: "SaveMapping",
                table: "WireMockServerModel");

            migrationBuilder.DropColumn(
                name: "SaveMappingForStatusCodePattern",
                table: "WireMockServerModel");

            migrationBuilder.DropColumn(
                name: "SaveMappingToFile",
                table: "WireMockServerModel");

            migrationBuilder.DropColumn(
                name: "StartAdminInterface",
                table: "WireMockServerModel");

            migrationBuilder.DropColumn(
                name: "Urls",
                table: "WireMockServerModel");
        }
    }
}
