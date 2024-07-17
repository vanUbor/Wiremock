using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WireMock.Migrations
{
    /// <inheritdoc />
    public partial class AddedGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "WireMockServerMapping",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "WireMockServerMapping");
        }
    }
}
