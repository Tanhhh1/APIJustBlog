using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "CreatedOn", "Description", "Name", "UpdatedOn", "UrlSlug" },
                values: new object[,]
                {
                    { 1, null, "Tin tức công nghệ mới nhất", "Công nghệ", null, "cong-nghe" },
                    { 2, null, "Khám phá và nghiên cứu khoa học", "Khoa học", null, "khoa-hoc" },
                    { 3, null, "Tin tức về phim ảnh, âm nhạc", "Giải trí", null, "giai-tri" },
                    { 4, null, "Tin tức tài chính và kinh doanh", "Kinh tế", null, "kinh-te" },
                    { 5, null, "Cập nhật các sự kiện thể thao mới nhất", "Thể thao", null, "the-thao" }
                });

            migrationBuilder.InsertData(
                table: "Tag",
                columns: new[] { "Id", "CreatedOn", "Description", "Name", "UpdatedOn", "UrlSlug" },
                values: new object[,]
                {
                    { 1, null, "Trí tuệ nhân tạo", "AI", null, "ai" },
                    { 2, null, "Học máy và ứng dụng", "Machine Learning", null, "machine-learning" },
                    { 3, null, "Khởi nghiệp sáng tạo", "Startup", null, "startup" },
                    { 4, null, "Công nghệ chuỗi khối", "Blockchain", null, "blockchain" },
                    { 5, null, "Bài viết về văn hóa và xã hội", "Văn hóa", null, "van-hoa" }
                });

            migrationBuilder.InsertData(
                table: "Post",
                columns: new[] { "Id", "CategoryId", "CreatedOn", "Description", "Meta", "Modified", "PostedOn", "Published", "ShortDescription", "Title", "UpdatedOn", "UrlSlug" },
                values: new object[,]
                {
                    { 1, 1, null, "AI đang dần trở thành một phần không thể thiếu...", "AI, công nghệ, tương lai", null, new DateTime(2025, 11, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Cách AI tác động đến các ngành nghề trong tương lai.", "AI thay đổi thế giới như thế nào", null, "ai-thay-doi-the-gioi" },
                    { 2, 2, null, "Kính viễn vọng James Webb giúp con người nhìn xa hơn...", "khoa học, vũ trụ", null, new DateTime(2025, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Các nhà khoa học đã phát hiện hành tinh mới...", "Khám phá vũ trụ với kính viễn vọng mới", null, "vu-tru-voi-kinh-vien-vong-moi" },
                    { 3, 4, null, "Nhiều startup công nghệ ra đời, thu hút vốn đầu tư lớn...", "startup, kinh tế", null, new DateTime(2025, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Khởi nghiệp đang là xu hướng mạnh tại Việt Nam.", "Doanh nghiệp Việt và làn sóng Startup", null, "doanh-nghiep-viet-startup" },
                    { 4, 4, null, "Công nghệ chuỗi khối mang lại sự minh bạch và bảo mật...", "blockchain, tài chính", null, new DateTime(2025, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Blockchain đang thay đổi cách giao dịch tài chính.", "Xu hướng Blockchain trong tài chính", null, "blockchain-trong-tai-chinh" },
                    { 5, 5, null, "Các vận động viên Việt Nam đạt thành tích ấn tượng...", "thể thao, olympic", null, new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Việt Nam giành huy chương tại Olympic Paris.", "Olympic Paris 2024 - niềm tự hào thể thao Việt", null, "olympic-paris-2024" }
                });

            migrationBuilder.InsertData(
                table: "PostTagMap",
                columns: new[] { "PostId", "TagId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PostTagMap",
                keyColumns: new[] { "PostId", "TagId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "PostTagMap",
                keyColumns: new[] { "PostId", "TagId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "PostTagMap",
                keyColumns: new[] { "PostId", "TagId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "PostTagMap",
                keyColumns: new[] { "PostId", "TagId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "PostTagMap",
                keyColumns: new[] { "PostId", "TagId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "PostTagMap",
                keyColumns: new[] { "PostId", "TagId" },
                keyValues: new object[] { 5, 5 });

            migrationBuilder.DeleteData(
                table: "Post",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Post",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Post",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Post",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Post",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
