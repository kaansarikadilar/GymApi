using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymApi.Migrations
{
    /// <inheritdoc />
    public partial class Members : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_AspNetUsers_AssignedTrainerId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_AppUserId",
                table: "Members");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Members",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Members",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Members_AppUserId",
                table: "Members",
                column: "AppUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_AspNetUsers_AssignedTrainerId",
                table: "Members",
                column: "AssignedTrainerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_AspNetUsers_AssignedTrainerId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_AppUserId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Members");

            migrationBuilder.CreateIndex(
                name: "IX_Members_AppUserId",
                table: "Members",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_AspNetUsers_AssignedTrainerId",
                table: "Members",
                column: "AssignedTrainerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
