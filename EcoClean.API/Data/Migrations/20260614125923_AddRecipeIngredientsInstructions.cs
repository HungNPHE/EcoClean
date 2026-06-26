using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EcoClean.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeIngredientsInstructions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ingredients",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "Ingredients", "Instructions", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1106), "Salad thanh mát với ức gà nướng mềm, giàu protein, thích hợp cho bữa trưa eat clean.", "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=400", "[\"200g ức gà\",\"1 bát rau xà lách\",\"1/2 quả cà chua\",\"1/2 quả dưa leo\",\"1 muỗng dầu olive\",\"1 muỗng nước cốt chanh\",\"Muối tiêu vừa đủ\"]", "[\"Ức gà ướp muối, tiêu, tỏi 15 phút\",\"Nướng gà ở 180°C khoảng 20 phút đến chín\",\"Rau xà lách rửa sạch, để ráo\",\"Cà chua và dưa leo thái lát\",\"Pha dressing: dầu olive + nước cốt chanh + muối tiêu\",\"Trộn rau + rau củ, xếp gà lên trên, rưới dressing\"]", 20 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "Ingredients", "Instructions", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1109), "Bát yến mạch ấm áp với trái cây tươi, cung cấp năng lượng bền vững cho buổi sáng.", "https://images.unsplash.com/photo-1517673400267-0251440c45dc?w=400", "[\"50g yến mạch cán dẹt\",\"200ml sữa tươi không đường\",\"1/2 quả chuối\",\"Vài quả dâu tây\",\"1 muỗng mật ong\",\"1 muỗng hạt chia\"]", "[\"Đun sữa tươi đến khi ấm (không sôi)\",\"Cho yến mạch vào, khuấy đều, nấu 3-5 phút\",\"Đổ ra bát, để nguội bớt\",\"Cắt chuối lát, dâu tây đôi\",\"Xếp trái cây lên trên, rưới mật ong, rắc hạt chia\"]", 10 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "Ingredients", "Instructions", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1111), "Cơm gạo lứt kết hợp cá hồi áp chảo giàu omega-3, bữa trưa cân bằng dinh dưỡng.", "https://images.unsplash.com/photo-1467003909585-2f8a72700288?w=400", "[\"150g cá hồi phi lê\",\"1 chén gạo lứt\",\"1/2 chén bông cải xanh\",\"1 muỗng dầu olive\",\"1 tép tỏi\",\"Muối, tiêu, nước tương\"]", "[\"Vo gạo lứt, nấu cơm như bình thường (thêm nước hơn gạo trắng)\",\"Cá hồi thấm khô, ướp muối tiêu\",\"Áp chảo cá hồi với dầu olive 3-4 phút mỗi mặt\",\"Xào bông cải xanh với tỏi phi vàng\",\"Dọn cơm ra đĩa, xếp cá hồi và rau cạnh\"]", 30 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "Ingredients", "Instructions", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1113), "Sinh tố bơ chuối béo ngậy, cung cấp chất béo lành mạnh và năng lượng nhanh.", "https://images.unsplash.com/photo-1611601322175-ef8ec8c3e978?w=400", "[\"1/2 quả bơ chín\",\"1 quả chuối đông lạnh\",\"200ml sữa hạnh nhân\",\"1 muỗng protein whey (tùy chọn)\",\"Đá viên\",\"Mật ong\"]", "[\"Cho tất cả nguyên liệu vào máy xay\",\"Xay nhuyễn đến khi mịn\",\"Nếm thử, thêm mật ong nếu cần ngọt hơn\",\"Rót ra ly, thưởng thức ngay\"]", 5 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "Ingredients", "Instructions", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1115), "Bữa sáng nhanh gọn với trứng bác và rau cải, ít carb, nhiều protein.", "https://images.unsplash.com/photo-1525351484163-7529414344d8?w=400", "[\"3 quả trứng gà\",\"1 bó cải bó xôi\",\"1/2 quả ớt chuông\",\"1 muỗng dầu olive\",\"Muối, tiêu\",\"Vài nhánh hành lá\"]", "[\"Đánh tan trứng với chút muối tiêu\",\"Phi hành lá với dầu olive\",\"Cho ớt chuông thái hạt lựu vào xào 1 phút\",\"Đổ trứng vào, khuấy nhẹ đến khi trứng vừa chín\",\"Cho rau cải vào, đảo đều, tắt bếp\"]", 10 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "Ingredients", "Instructions", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1117), "Gà luộc thanh đạm ăn cùng rau hấp, bữa tối eat clean lý tưởng.", "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?w=400", "[\"300g ức gà\",\"1 cây bông cải xanh\",\"2 cà rốt\",\"1/2 bắp cải\",\"Gừng, sả\",\"Nước mắm, chanh\"]", "[\"Luộc gà với gừng và sả đến chín khoảng 20-25 phút\",\"Bông cải, cà rốt, bắp cải hấp chín tới (khoảng 8-10 phút)\",\"Gà nguội bớt thì xé sợi\",\"Pha nước chấm: nước mắm + chanh + tỏi ớt\",\"Dọn ra đĩa, ăn kèm rau và nước chấm\"]", 35 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "Ingredients", "Instructions", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1119), "Yến mạch ngâm đêm tiện lợi, sáng dậy có ngay bữa ăn giàu chất xơ.", "https://images.unsplash.com/photo-1574269909862-7e1d70bb8078?w=400", "[\"50g yến mạch\",\"150ml sữa chua không đường\",\"100ml sữa tươi\",\"1 muỗng hạt chia\",\"1 muỗng mật ong\",\"Trái cây tùy thích\"]", "[\"Trộn yến mạch, sữa chua, sữa tươi và hạt chia trong hũ thủy tinh\",\"Khuấy đều, thêm mật ong\",\"Đậy nắp, để tủ lạnh qua đêm (ít nhất 6 tiếng)\",\"Sáng hôm sau lấy ra, thêm trái cây tươi lên trên\"]", 5 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "Ingredients", "Instructions", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1120), "Bowl quinoa kết hợp cá ngừ đóng hộp và rau củ đa màu sắc.", "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400", "[\"1/2 chén quinoa\",\"1 hộp cá ngừ trong nước\",\"1/2 quả bơ\",\"1 bắp ngô luộc\",\"Rau xà lách\",\"Dầu olive, chanh, muối tiêu\"]", "[\"Nấu quinoa với nước theo tỉ lệ 1:2 trong 15 phút\",\"Cá ngừ ráo nước, tơi ra\",\"Bơ thái hạt lựu, bắp ngô tách hạt\",\"Xếp quinoa vào bowl, đặt các topping lên trên\",\"Rưới dầu olive và nước cốt chanh, nêm muối tiêu\"]", 25 });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "Calories", "Carb", "Category", "CreatedAt", "Description", "Fat", "Fiber", "ImageUrl", "Ingredients", "Instructions", "IsActive", "Name", "PrepTimeMin", "Protein", "Tags" },
                values: new object[,]
                {
                    { 9, 350, 45.0, "Lunch", new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1122), "Phở gà truyền thống nấu ít dầu mỡ, thanh nhẹ nhưng vẫn đậm đà hương vị.", 5.0, 2.0, "https://images.unsplash.com/photo-1582878826629-33b2ad425bca?w=400", "[\"200g ức gà\",\"1 bó bánh phở\",\"1 củ hành tây\",\"1 nhánh gừng nướng\",\"Hành lá, ngò rí\",\"Giá đỗ, rau húng quế\",\"Nước mắm, muối\"]", "[\"Nướng hành tây và gừng trên lửa đến khi thơm, cháy vỏ\",\"Luộc gà trong 2 lít nước với hành và gừng 30 phút\",\"Vớt gà ra, thái lát; lọc nước dùng\",\"Nêm nước dùng với muối, nước mắm vừa ăn\",\"Trụng bánh phở, xếp vào tô\",\"Đổ nước dùng sôi, xếp gà, hành lá, ngò lên trên\",\"Ăn kèm giá đỗ và húng quế\"]", true, "Phở gà ít béo", 45, 30.0, "[\"vietnamese\",\"low-fat\"]" },
                    { 10, 380, 50.0, "Lunch", new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1124), "Bún bò Huế phiên bản eat clean, ít mỡ, nước dùng đậm đà từ sả và mắm ruốc.", 6.0, 3.0, "https://images.unsplash.com/photo-1562802378-063ec186a863?w=400", "[\"200g thịt bò nạm\",\"1 bó bún\",\"2 cây sả\",\"1 muỗng mắm ruốc Huế\",\"1 muỗng ớt sa tế\",\"Hành lá, ngò gai, bắp chuối bào\",\"Muối, đường\"]", "[\"Hầm thịt bò với sả và muối khoảng 40 phút đến mềm\",\"Pha mắm ruốc với nước ấm, lọc lấy nước trong\",\"Cho nước mắm ruốc vào nồi nước dùng, nêm vừa ăn\",\"Thêm sa tế theo khẩu vị\",\"Trụng bún, xếp vào tô cùng thịt bò thái lát\",\"Chan nước dùng sôi, thêm hành lá và ngò gai\"]", true, "Bún bò Huế eat clean", 50, 28.0, "[\"vietnamese\",\"spicy\"]" },
                    { 11, 120, 20.0, "Dinner", new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1126), "Canh rau củ thanh mát, giàu chất xơ, hỗ trợ tiêu hoá và thanh nhiệt cơ thể.", 2.0, 6.0, "https://images.unsplash.com/photo-1547592180-85f173990554?w=400", "[\"1 củ cà rốt\",\"200g bí đao\",\"1 bó rau ngót\",\"2 quả cà chua\",\"1 muỗng nước mắm\",\"Hành lá, muối\"]", "[\"Cà rốt và bí đao gọt vỏ, thái miếng vừa ăn\",\"Cà chua bổ múi cau\",\"Đun sôi 600ml nước, cho cà rốt vào trước (cứng hơn)\",\"Sau 5 phút thêm bí đao và cà chua\",\"Nêm nước mắm và muối vừa ăn\",\"Cho rau ngót vào, đun sôi lại 1-2 phút, tắt bếp\",\"Rắc hành lá lên trên\"]", true, "Canh rau củ thanh nhiệt", 20, 5.0, "[\"vietnamese\",\"detox\",\"vegan\"]" },
                    { 12, 340, 40.0, "Breakfast", new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1128), "Bánh mì ngũ cốc kẹp ức gà áp chảo và rau củ, bữa sáng no lâu và dinh dưỡng.", 7.0, 5.0, "https://images.unsplash.com/photo-1553909489-cd47e0907980?w=400", "[\"2 lát bánh mì ngũ cốc\",\"120g ức gà\",\"Vài lá xà lách\",\"1/2 quả cà chua\",\"1/4 quả dưa leo\",\"1 muỗng hummus hoặc sốt mù tạt mật ong\",\"Muối tiêu\"]", "[\"Ức gà ướp muối tiêu, áp chảo đến chín vàng\",\"Bánh mì nướng giòn trên chảo khô\",\"Phết hummus lên một mặt bánh\",\"Xếp xà lách, cà chua, dưa leo và gà lên bánh\",\"Đậy lát bánh còn lại, cắt đôi và thưởng thức\"]", true, "Bánh mì ức gà ngũ cốc", 15, 28.0, "[\"high-protein\",\"wholegrains\"]" },
                    { 13, 180, 28.0, "Snack", new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1165), "Súp bí đỏ mịn màng, thơm ngon, giàu vitamin A và chất chống oxy hóa.", 6.0, 4.0, "https://images.unsplash.com/photo-1476718406336-bb5a9690ee2a?w=400", "[\"300g bí đỏ\",\"1 củ hành tây nhỏ\",\"2 tép tỏi\",\"300ml nước dùng rau củ\",\"100ml sữa dừa\",\"Hạt bí rang, dầu olive\",\"Muối, tiêu, nhục đậu khấu\"]", "[\"Bí đỏ gọt vỏ, cắt miếng; hành tây thái nhỏ\",\"Phi hành tây và tỏi với dầu olive đến mềm\",\"Cho bí đỏ vào, đảo đều\",\"Đổ nước dùng vào, nấu 15 phút đến bí mềm\",\"Dùng máy xay sinh tố xay nhuyễn\",\"Cho sữa dừa vào, đun ấm lại, nêm muối tiêu\",\"Rắc hạt bí rang và vài giọt dầu olive lên trên\"]", true, "Súp bí đỏ hạt bí", 25, 4.0, "[\"vegan\",\"antioxidant\"]" },
                    { 14, 220, 4.0, "Dinner", new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1167), "Cá basa hấp gừng hành thanh đạm, giữ trọn dưỡng chất, phù hợp ăn kiêng.", 8.0, 1.0, "https://images.unsplash.com/photo-1519708227418-c8fd9a32b7a2?w=400", "[\"300g cá basa phi lê\",\"1 nhánh gừng\",\"3 nhánh hành lá\",\"2 muỗng nước tương\",\"1 muỗng dầu mè\",\"1 muỗng dầu ăn\",\"Tiêu xay\"]", "[\"Cá basa rửa sạch, thấm khô, xếp vào đĩa hấp\",\"Gừng thái chỉ, rải đều lên cá\",\"Hấp cá trong 12-15 phút đến chín\",\"Đun nóng dầu ăn đến khói\",\"Lấy cá ra, rắc hành lá và tiêu\",\"Rưới nước tương và dầu mè lên cá\",\"Đổ dầu nóng lên để dậy mùi thơm\"]", true, "Cá basa hấp gừng hành", 20, 32.0, "[\"vietnamese\",\"lean\",\"steamed\"]" },
                    { 15, 95, 12.0, "Snack", new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1169), "Salad dưa chuột giòn mát, ít calo, phù hợp ăn vặt lành mạnh bất cứ lúc nào.", 4.0, 2.0, "https://images.unsplash.com/photo-1540420773420-3366772f4999?w=400", "[\"2 quả dưa chuột\",\"1/2 quả hành tây tím\",\"Vài cọng rau mùi\",\"2 muỗng giấm gạo\",\"1 muỗng dầu mè\",\"1 muỗng nước mắm\",\"Ớt, tỏi băm\",\"Mè rang\"]", "[\"Dưa chuột thái lát mỏng hoặc gọt sọc rồi thái\",\"Hành tây tím thái lát mỏng, ngâm nước đá 5 phút bớt hăng\",\"Pha nước trộn: giấm + dầu mè + nước mắm + tỏi + ớt\",\"Trộn dưa chuột và hành tây với nước trộn\",\"Để tủ lạnh 15 phút cho ngấm\",\"Rắc mè rang và rau mùi trước khi ăn\"]", true, "Salad dưa chuột thanh mát", 10, 3.0, "[\"vegan\",\"low-cal\",\"refreshing\"]" },
                    { 16, 310, 6.0, "Dinner", new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1171), "Thịt heo nạc kho tiêu đậm đà, ăn với cơm gạo lứt hoặc cơm cauli.", 15.0, 0.5, "https://images.unsplash.com/photo-1559847844-5315695dadae?w=400", "[\"300g thịt heo nạc vai\",\"2 muỗng nước mắm\",\"1 muỗng nước tương\",\"1 muỗng đường dừa\",\"1 muỗng tiêu xay\",\"3 tép tỏi\",\"2 muỗng nước dừa\",\"Hành lá\"]", "[\"Thịt heo thái miếng 2cm, rửa sạch thấm khô\",\"Ướp với nước mắm, nước tương, đường, tiêu, tỏi 20 phút\",\"Cho thịt vào nồi, đảo đều trên lửa vừa\",\"Thêm nước dừa, kho liu riu 20-25 phút\",\"Nêm lại gia vị, kho đến khi nước cạn sệt\",\"Rắc hành lá và tiêu thêm trước khi dọn\"]", true, "Thịt heo nạc kho tiêu", 35, 36.0, "[\"vietnamese\",\"high-protein\"]" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DropColumn(
                name: "Ingredients",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "Recipes");

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1293), null, null, null });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1295), null, null, null });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1297), null, null, null });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1298), null, null, null });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1299), null, null, null });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1301), null, null, null });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1302), null, null, null });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "Description", "ImageUrl", "PrepTimeMin" },
                values: new object[] { new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1303), null, null, null });
        }
    }
}
