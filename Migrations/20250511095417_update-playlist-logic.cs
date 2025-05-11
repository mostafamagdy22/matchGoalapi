using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchGoalAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateplaylistlogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayLists_AspNetUsers_UserID",
                table: "PlayLists");

            migrationBuilder.DropTable(
                name: "PlayListMatches");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "PlayLists");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "PlayLists",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "PlayLists",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "PlayLists",
                newName: "AddedDate");

            migrationBuilder.RenameIndex(
                name: "IX_PlayLists_UserID",
                table: "PlayLists",
                newName: "IX_PlayLists_UserId");

            migrationBuilder.AddColumn<int>(
                name: "MatchId",
                table: "PlayLists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PlayLists_MatchId",
                table: "PlayLists",
                column: "MatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayLists_AspNetUsers_UserId",
                table: "PlayLists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayLists_Matches_MatchId",
                table: "PlayLists",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayLists_AspNetUsers_UserId",
                table: "PlayLists");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayLists_Matches_MatchId",
                table: "PlayLists");

            migrationBuilder.DropIndex(
                name: "IX_PlayLists_MatchId",
                table: "PlayLists");

            migrationBuilder.DropColumn(
                name: "MatchId",
                table: "PlayLists");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PlayLists",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PlayLists",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "AddedDate",
                table: "PlayLists",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_PlayLists_UserId",
                table: "PlayLists",
                newName: "IX_PlayLists_UserID");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "PlayLists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PlayListMatches",
                columns: table => new
                {
                    PlayListID = table.Column<int>(type: "int", nullable: false),
                    MatchID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayListMatches", x => new { x.PlayListID, x.MatchID });
                    table.ForeignKey(
                        name: "FK_PlayListMatches_Matches_MatchID",
                        column: x => x.MatchID,
                        principalTable: "Matches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayListMatches_PlayLists_PlayListID",
                        column: x => x.PlayListID,
                        principalTable: "PlayLists",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayListMatches_MatchID",
                table: "PlayListMatches",
                column: "MatchID");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayLists_AspNetUsers_UserID",
                table: "PlayLists",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
