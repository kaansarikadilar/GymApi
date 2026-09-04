using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymApi.Migrations.BarcodeDb
{
    /// <inheritdoc />
    public partial class Loop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MemberCode",
                table: "Barcodes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MemberName",
                table: "Barcodes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberCode",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "MemberName",
                table: "Barcodes");
        }
    }
}
