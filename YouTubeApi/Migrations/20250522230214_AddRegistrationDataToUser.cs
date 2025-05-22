using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubeApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationDataToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegistrationChannelId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationEmail",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationPassword",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationTokenExpiry",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationUsername",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegistrationYear",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationChannelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RegistrationEmail",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RegistrationPassword",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RegistrationTokenExpiry",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RegistrationUsername",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RegistrationYear",
                table: "AspNetUsers");
        }
    }
}
