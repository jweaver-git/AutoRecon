using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoRecon.Migrations
{
    /// <inheritdoc />
    public partial class AddTrueRawNmapOutput : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrueRawNmapOutput",
                table: "Scans",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrueRawNmapOutput",
                table: "Scans");
        }
    }
}
