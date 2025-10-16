using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoryGame.Migrations
{
    /// <inheritdoc />
    public partial class addedFirstScene : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choice_Scenes_NextSceneId",
                table: "Choice");

            migrationBuilder.DropForeignKey(
                name: "FK_Choice_Scenes_ThisSceneId",
                table: "Choice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Choice",
                table: "Choice");

            migrationBuilder.RenameTable(
                name: "Choice",
                newName: "Choices");

            migrationBuilder.RenameIndex(
                name: "IX_Choice_ThisSceneId",
                table: "Choices",
                newName: "IX_Choices_ThisSceneId");

            migrationBuilder.RenameIndex(
                name: "IX_Choice_NextSceneId",
                table: "Choices",
                newName: "IX_Choices_NextSceneId");

            migrationBuilder.AddColumn<bool>(
                name: "IsFirstScene",
                table: "Scenes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "NextSceneId",
                table: "Choices",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Choices",
                table: "Choices",
                column: "ChoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Choices_Scenes_NextSceneId",
                table: "Choices",
                column: "NextSceneId",
                principalTable: "Scenes",
                principalColumn: "SceneId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Choices_Scenes_ThisSceneId",
                table: "Choices",
                column: "ThisSceneId",
                principalTable: "Scenes",
                principalColumn: "SceneId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choices_Scenes_NextSceneId",
                table: "Choices");

            migrationBuilder.DropForeignKey(
                name: "FK_Choices_Scenes_ThisSceneId",
                table: "Choices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Choices",
                table: "Choices");

            migrationBuilder.DropColumn(
                name: "IsFirstScene",
                table: "Scenes");

            migrationBuilder.RenameTable(
                name: "Choices",
                newName: "Choice");

            migrationBuilder.RenameIndex(
                name: "IX_Choices_ThisSceneId",
                table: "Choice",
                newName: "IX_Choice_ThisSceneId");

            migrationBuilder.RenameIndex(
                name: "IX_Choices_NextSceneId",
                table: "Choice",
                newName: "IX_Choice_NextSceneId");

            migrationBuilder.AlterColumn<int>(
                name: "NextSceneId",
                table: "Choice",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Choice",
                table: "Choice",
                column: "ChoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Choice_Scenes_NextSceneId",
                table: "Choice",
                column: "NextSceneId",
                principalTable: "Scenes",
                principalColumn: "SceneId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Choice_Scenes_ThisSceneId",
                table: "Choice",
                column: "ThisSceneId",
                principalTable: "Scenes",
                principalColumn: "SceneId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
