using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameDbSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Category_CategoryId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTagMap_Post_PostId",
                table: "PostTagMap");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTagMap_Tag_TagId",
                table: "PostTagMap");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tag",
                table: "Tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostTagMap",
                table: "PostTagMap");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Post",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "Tag",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "PostTagMap",
                newName: "PostTagMaps");

            migrationBuilder.RenameTable(
                name: "Post",
                newName: "Posts");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_Tag_UrlSlug",
                table: "Tags",
                newName: "IX_Tags_UrlSlug");

            migrationBuilder.RenameIndex(
                name: "IX_PostTagMap_TagId",
                table: "PostTagMaps",
                newName: "IX_PostTagMaps_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_UrlSlug",
                table: "Posts",
                newName: "IX_Posts_UrlSlug");

            migrationBuilder.RenameIndex(
                name: "IX_Post_CategoryId",
                table: "Posts",
                newName: "IX_Posts_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Category_UrlSlug",
                table: "Categories",
                newName: "IX_Categories_UrlSlug");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostTagMaps",
                table: "PostTagMaps",
                columns: new[] { "PostId", "TagId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Categories_CategoryId",
                table: "Posts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTagMaps_Posts_PostId",
                table: "PostTagMaps",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTagMaps_Tags_TagId",
                table: "PostTagMaps",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Categories_CategoryId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTagMaps_Posts_PostId",
                table: "PostTagMaps");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTagMaps_Tags_TagId",
                table: "PostTagMaps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostTagMaps",
                table: "PostTagMaps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "Tag");

            migrationBuilder.RenameTable(
                name: "PostTagMaps",
                newName: "PostTagMap");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Post");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_UrlSlug",
                table: "Tag",
                newName: "IX_Tag_UrlSlug");

            migrationBuilder.RenameIndex(
                name: "IX_PostTagMaps_TagId",
                table: "PostTagMap",
                newName: "IX_PostTagMap_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UrlSlug",
                table: "Post",
                newName: "IX_Post_UrlSlug");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_CategoryId",
                table: "Post",
                newName: "IX_Post_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_UrlSlug",
                table: "Category",
                newName: "IX_Category_UrlSlug");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tag",
                table: "Tag",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostTagMap",
                table: "PostTagMap",
                columns: new[] { "PostId", "TagId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post",
                table: "Post",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Category_CategoryId",
                table: "Post",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTagMap_Post_PostId",
                table: "PostTagMap",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTagMap_Tag_TagId",
                table: "PostTagMap",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
