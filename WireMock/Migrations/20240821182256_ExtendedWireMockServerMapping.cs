using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WireMock.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedWireMockServerMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartAdminInterface",
                table: "WireMockServerModel");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "WireMockServerMapping",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "WireMockServerMapping",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "WireMockServerMapping");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "WireMockServerMapping");

            migrationBuilder.AddColumn<bool>(
                name: "StartAdminInterface",
                table: "WireMockServerModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
