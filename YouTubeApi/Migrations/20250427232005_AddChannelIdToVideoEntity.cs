using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubeApi.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelIdToVideoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Channeled",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Channeled",
                table: "Videos");
        }
    }
}
