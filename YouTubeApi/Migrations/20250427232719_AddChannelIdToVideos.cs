using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubeApi.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelIdToVideos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Channeled",
                table: "Videos",
                newName: "ChannelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChannelId",
                table: "Videos",
                newName: "Channeled");
        }
    }
}
