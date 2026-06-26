using EcoClean.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoClean.API.Data;

public class EcoCleanDbContext : DbContext
{
    public EcoCleanDbContext(DbContextOptions<EcoCleanDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<MealPlan> MealPlans => Set<MealPlan>();
    public DbSet<WeightLog> WeightLogs => Set<WeightLog>();
    public DbSet<FoodScan> FoodScans => Set<FoodScan>();
    public DbSet<ChatHistory> ChatHistories => Set<ChatHistory>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<AIMealPlan> AIMealPlans => Set<AIMealPlan>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // User
        mb.Entity<User>(e => {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Role).HasMaxLength(20);
        });

        // UserProfile 1-1
        mb.Entity<UserProfile>(e => {
            e.HasOne(p => p.User).WithOne(u => u.Profile)
             .HasForeignKey<UserProfile>(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Recipe
        mb.Entity<Recipe>(e => {
            e.Property(r => r.Category).HasMaxLength(50);
        });

        // MealPlan
        mb.Entity<MealPlan>(e => {
            e.HasIndex(m => new { m.UserId, m.PlanDate });
            e.HasOne(m => m.User).WithMany(u => u.MealPlans)
             .HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(m => m.Recipe).WithMany()
             .HasForeignKey(m => m.RecipeId).OnDelete(DeleteBehavior.SetNull);
        });

        // WeightLog
        mb.Entity<WeightLog>(e => {
            e.HasIndex(w => new { w.UserId, w.LogDate });
            e.HasOne(w => w.User).WithMany(u => u.WeightLogs)
             .HasForeignKey(w => w.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // FoodScan
        mb.Entity<FoodScan>(e => {
            e.HasOne(f => f.User).WithMany(u => u.FoodScans)
             .HasForeignKey(f => f.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // ChatHistory — bảng trong DB tên là "ChatHistory" (số ít)
        mb.Entity<ChatHistory>(e => {
            e.ToTable("ChatHistory");
            e.HasIndex(c => new { c.UserId, c.SessionId });
            e.HasOne(c => c.User).WithMany(u => u.ChatHistories)
             .HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Subscription
        mb.Entity<Subscription>(e => {
            e.Property(s => s.Amount).HasColumnType("decimal(10,2)");
            e.HasOne(s => s.User).WithMany(u => u.Subscriptions)
             .HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // AIMealPlan
        mb.Entity<AIMealPlan>(e => {
            e.HasOne(a => a.User).WithMany(u => u.AIMealPlans)
             .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Seed Recipes
        mb.Entity<Recipe>().HasData(
            new Recipe { Id=1,  Name="Salad ức gà nướng",    Calories=320, Protein=35.0, Carb=12.0, Fat=14.0, Fiber=4.0,  Category="Lunch",     Tags="[\"high-protein\",\"low-carb\"]",    IsActive=true, PrepTimeMin=20, ImageUrl="https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=400",   Description="Salad thanh mát với ức gà nướng mềm, giàu protein, thích hợp cho bữa trưa eat clean.", Ingredients="[\"200g ức gà\",\"1 bát rau xà lách\",\"1/2 quả cà chua\",\"1/2 quả dưa leo\",\"1 muỗng dầu olive\",\"1 muỗng nước cốt chanh\",\"Muối tiêu vừa đủ\"]", Instructions="[\"Ức gà ướp muối, tiêu, tỏi 15 phút\",\"Nướng gà ở 180°C khoảng 20 phút đến chín\",\"Rau xà lách rửa sạch, để ráo\",\"Cà chua và dưa leo thái lát\",\"Pha dressing: dầu olive + nước cốt chanh + muối tiêu\",\"Trộn rau + rau củ, xếp gà lên trên, rưới dressing\"]" },
            new Recipe { Id=2,  Name="Yến mạch trái cây",    Calories=280, Protein=8.0,  Carb=52.0, Fat=5.0,  Fiber=6.0,  Category="Breakfast", Tags="[\"fiber\",\"energy\"]",             IsActive=true, PrepTimeMin=10, ImageUrl="https://images.unsplash.com/photo-1517673400267-0251440c45dc?w=400",   Description="Bát yến mạch ấm áp với trái cây tươi, cung cấp năng lượng bền vững cho buổi sáng.", Ingredients="[\"50g yến mạch cán dẹt\",\"200ml sữa tươi không đường\",\"1/2 quả chuối\",\"Vài quả dâu tây\",\"1 muỗng mật ong\",\"1 muỗng hạt chia\"]", Instructions="[\"Đun sữa tươi đến khi ấm (không sôi)\",\"Cho yến mạch vào, khuấy đều, nấu 3-5 phút\",\"Đổ ra bát, để nguội bớt\",\"Cắt chuối lát, dâu tây đôi\",\"Xếp trái cây lên trên, rưới mật ong, rắc hạt chia\"]" },
            new Recipe { Id=3,  Name="Cơm gạo lứt cá hồi",   Calories=450, Protein=42.0, Carb=48.0, Fat=12.0, Fiber=5.0,  Category="Lunch",     Tags="[\"omega3\",\"balanced\"]",          IsActive=true, PrepTimeMin=30, ImageUrl="https://images.unsplash.com/photo-1467003909585-2f8a72700288?w=400",   Description="Cơm gạo lứt kết hợp cá hồi áp chảo giàu omega-3, bữa trưa cân bằng dinh dưỡng.", Ingredients="[\"150g cá hồi phi lê\",\"1 chén gạo lứt\",\"1/2 chén bông cải xanh\",\"1 muỗng dầu olive\",\"1 tép tỏi\",\"Muối, tiêu, nước tương\"]", Instructions="[\"Vo gạo lứt, nấu cơm như bình thường (thêm nước hơn gạo trắng)\",\"Cá hồi thấm khô, ướp muối tiêu\",\"Áp chảo cá hồi với dầu olive 3-4 phút mỗi mặt\",\"Xào bông cải xanh với tỏi phi vàng\",\"Dọn cơm ra đĩa, xếp cá hồi và rau cạnh\"]" },
            new Recipe { Id=4,  Name="Smoothie bơ chuối",     Calories=380, Protein=12.0, Carb=38.0, Fat=20.0, Fiber=6.0,  Category="Snack",     Tags="[\"smoothie\",\"healthy-fat\"]",     IsActive=true, PrepTimeMin=5,  ImageUrl="https://images.unsplash.com/photo-1611601322175-ef8ec8c3e978?w=400",   Description="Sinh tố bơ chuối béo ngậy, cung cấp chất béo lành mạnh và năng lượng nhanh.", Ingredients="[\"1/2 quả bơ chín\",\"1 quả chuối đông lạnh\",\"200ml sữa hạnh nhân\",\"1 muỗng protein whey (tùy chọn)\",\"Đá viên\",\"Mật ong\"]", Instructions="[\"Cho tất cả nguyên liệu vào máy xay\",\"Xay nhuyễn đến khi mịn\",\"Nếm thử, thêm mật ong nếu cần ngọt hơn\",\"Rót ra ly, thưởng thức ngay\"]" },
            new Recipe { Id=5,  Name="Trứng bác rau cải",     Calories=210, Protein=18.0, Carb=6.0,  Fat=13.0, Fiber=2.0,  Category="Breakfast", Tags="[\"low-carb\",\"quick\"]",           IsActive=true, PrepTimeMin=10, ImageUrl="https://images.unsplash.com/photo-1525351484163-7529414344d8?w=400",   Description="Bữa sáng nhanh gọn với trứng bác và rau cải, ít carb, nhiều protein.", Ingredients="[\"3 quả trứng gà\",\"1 bó cải bó xôi\",\"1/2 quả ớt chuông\",\"1 muỗng dầu olive\",\"Muối, tiêu\",\"Vài nhánh hành lá\"]", Instructions="[\"Đánh tan trứng với chút muối tiêu\",\"Phi hành lá với dầu olive\",\"Cho ớt chuông thái hạt lựu vào xào 1 phút\",\"Đổ trứng vào, khuấy nhẹ đến khi trứng vừa chín\",\"Cho rau cải vào, đảo đều, tắt bếp\"]" },
            new Recipe { Id=6,  Name="Gà luộc rau hấp",       Calories=290, Protein=40.0, Carb=14.0, Fat=6.0,  Fiber=5.0,  Category="Dinner",    Tags="[\"lean\",\"clean\"]",               IsActive=true, PrepTimeMin=35, ImageUrl="https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?w=400",   Description="Gà luộc thanh đạm ăn cùng rau hấp, bữa tối eat clean lý tưởng.", Ingredients="[\"300g ức gà\",\"1 cây bông cải xanh\",\"2 cà rốt\",\"1/2 bắp cải\",\"Gừng, sả\",\"Nước mắm, chanh\"]", Instructions="[\"Luộc gà với gừng và sả đến chín khoảng 20-25 phút\",\"Bông cải, cà rốt, bắp cải hấp chín tới (khoảng 8-10 phút)\",\"Gà nguội bớt thì xé sợi\",\"Pha nước chấm: nước mắm + chanh + tỏi ớt\",\"Dọn ra đĩa, ăn kèm rau và nước chấm\"]" },
            new Recipe { Id=7,  Name="Overnight oats",        Calories=310, Protein=14.0, Carb=44.0, Fat=8.0,  Fiber=8.0,  Category="Breakfast", Tags="[\"prep-ahead\",\"fiber\"]",         IsActive=true, PrepTimeMin=5,  ImageUrl="https://images.unsplash.com/photo-1574269909862-7e1d70bb8078?w=400",   Description="Yến mạch ngâm đêm tiện lợi, sáng dậy có ngay bữa ăn giàu chất xơ.", Ingredients="[\"50g yến mạch\",\"150ml sữa chua không đường\",\"100ml sữa tươi\",\"1 muỗng hạt chia\",\"1 muỗng mật ong\",\"Trái cây tùy thích\"]", Instructions="[\"Trộn yến mạch, sữa chua, sữa tươi và hạt chia trong hũ thủy tinh\",\"Khuấy đều, thêm mật ong\",\"Đậy nắp, để tủ lạnh qua đêm (ít nhất 6 tiếng)\",\"Sáng hôm sau lấy ra, thêm trái cây tươi lên trên\"]" },
            new Recipe { Id=8,  Name="Bowl cá ngừ quinoa",    Calories=400, Protein=38.0, Carb=42.0, Fat=10.0, Fiber=5.0,  Category="Dinner",    Tags="[\"complete-protein\",\"grain\"]",   IsActive=true, PrepTimeMin=25, ImageUrl="https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400",   Description="Bowl quinoa kết hợp cá ngừ đóng hộp và rau củ đa màu sắc.", Ingredients="[\"1/2 chén quinoa\",\"1 hộp cá ngừ trong nước\",\"1/2 quả bơ\",\"1 bắp ngô luộc\",\"Rau xà lách\",\"Dầu olive, chanh, muối tiêu\"]", Instructions="[\"Nấu quinoa với nước theo tỉ lệ 1:2 trong 15 phút\",\"Cá ngừ ráo nước, tơi ra\",\"Bơ thái hạt lựu, bắp ngô tách hạt\",\"Xếp quinoa vào bowl, đặt các topping lên trên\",\"Rưới dầu olive và nước cốt chanh, nêm muối tiêu\"]" },
            // ── Công thức Việt Nam mới ──────────────────────────
            new Recipe { Id=9,  Name="Phở gà ít béo",         Calories=350, Protein=30.0, Carb=45.0, Fat=5.0,  Fiber=2.0,  Category="Lunch",     Tags="[\"vietnamese\",\"low-fat\"]",       IsActive=true, PrepTimeMin=45, ImageUrl="https://images.unsplash.com/photo-1582878826629-33b2ad425bca?w=400",   Description="Phở gà truyền thống nấu ít dầu mỡ, thanh nhẹ nhưng vẫn đậm đà hương vị.", Ingredients="[\"200g ức gà\",\"1 bó bánh phở\",\"1 củ hành tây\",\"1 nhánh gừng nướng\",\"Hành lá, ngò rí\",\"Giá đỗ, rau húng quế\",\"Nước mắm, muối\"]", Instructions="[\"Nướng hành tây và gừng trên lửa đến khi thơm, cháy vỏ\",\"Luộc gà trong 2 lít nước với hành và gừng 30 phút\",\"Vớt gà ra, thái lát; lọc nước dùng\",\"Nêm nước dùng với muối, nước mắm vừa ăn\",\"Trụng bánh phở, xếp vào tô\",\"Đổ nước dùng sôi, xếp gà, hành lá, ngò lên trên\",\"Ăn kèm giá đỗ và húng quế\"]" },
            new Recipe { Id=10, Name="Bún bò Huế eat clean",  Calories=380, Protein=28.0, Carb=50.0, Fat=6.0,  Fiber=3.0,  Category="Lunch",     Tags="[\"vietnamese\",\"spicy\"]",         IsActive=true, PrepTimeMin=50, ImageUrl="https://images.unsplash.com/photo-1562802378-063ec186a863?w=400",   Description="Bún bò Huế phiên bản eat clean, ít mỡ, nước dùng đậm đà từ sả và mắm ruốc.", Ingredients="[\"200g thịt bò nạm\",\"1 bó bún\",\"2 cây sả\",\"1 muỗng mắm ruốc Huế\",\"1 muỗng ớt sa tế\",\"Hành lá, ngò gai, bắp chuối bào\",\"Muối, đường\"]", Instructions="[\"Hầm thịt bò với sả và muối khoảng 40 phút đến mềm\",\"Pha mắm ruốc với nước ấm, lọc lấy nước trong\",\"Cho nước mắm ruốc vào nồi nước dùng, nêm vừa ăn\",\"Thêm sa tế theo khẩu vị\",\"Trụng bún, xếp vào tô cùng thịt bò thái lát\",\"Chan nước dùng sôi, thêm hành lá và ngò gai\"]" },
            new Recipe { Id=11, Name="Canh rau củ thanh nhiệt",Calories=120, Protein=5.0,  Carb=20.0, Fat=2.0,  Fiber=6.0,  Category="Dinner",    Tags="[\"vietnamese\",\"detox\",\"vegan\"]",IsActive=true, PrepTimeMin=20, ImageUrl="https://images.unsplash.com/photo-1547592180-85f173990554?w=400",   Description="Canh rau củ thanh mát, giàu chất xơ, hỗ trợ tiêu hoá và thanh nhiệt cơ thể.", Ingredients="[\"1 củ cà rốt\",\"200g bí đao\",\"1 bó rau ngót\",\"2 quả cà chua\",\"1 muỗng nước mắm\",\"Hành lá, muối\"]", Instructions="[\"Cà rốt và bí đao gọt vỏ, thái miếng vừa ăn\",\"Cà chua bổ múi cau\",\"Đun sôi 600ml nước, cho cà rốt vào trước (cứng hơn)\",\"Sau 5 phút thêm bí đao và cà chua\",\"Nêm nước mắm và muối vừa ăn\",\"Cho rau ngót vào, đun sôi lại 1-2 phút, tắt bếp\",\"Rắc hành lá lên trên\"]" },
            new Recipe { Id=12, Name="Bánh mì ức gà ngũ cốc", Calories=340, Protein=28.0, Carb=40.0, Fat=7.0,  Fiber=5.0,  Category="Breakfast", Tags="[\"high-protein\",\"wholegrains\"]",  IsActive=true, PrepTimeMin=15, ImageUrl="https://images.unsplash.com/photo-1553909489-cd47e0907980?w=400",   Description="Bánh mì ngũ cốc kẹp ức gà áp chảo và rau củ, bữa sáng no lâu và dinh dưỡng.", Ingredients="[\"2 lát bánh mì ngũ cốc\",\"120g ức gà\",\"Vài lá xà lách\",\"1/2 quả cà chua\",\"1/4 quả dưa leo\",\"1 muỗng hummus hoặc sốt mù tạt mật ong\",\"Muối tiêu\"]", Instructions="[\"Ức gà ướp muối tiêu, áp chảo đến chín vàng\",\"Bánh mì nướng giòn trên chảo khô\",\"Phết hummus lên một mặt bánh\",\"Xếp xà lách, cà chua, dưa leo và gà lên bánh\",\"Đậy lát bánh còn lại, cắt đôi và thưởng thức\"]" },
            new Recipe { Id=13, Name="Súp bí đỏ hạt bí",      Calories=180, Protein=4.0,  Carb=28.0, Fat=6.0,  Fiber=4.0,  Category="Snack",     Tags="[\"vegan\",\"antioxidant\"]",        IsActive=true, PrepTimeMin=25, ImageUrl="https://images.unsplash.com/photo-1476718406336-bb5a9690ee2a?w=400",   Description="Súp bí đỏ mịn màng, thơm ngon, giàu vitamin A và chất chống oxy hóa.", Ingredients="[\"300g bí đỏ\",\"1 củ hành tây nhỏ\",\"2 tép tỏi\",\"300ml nước dùng rau củ\",\"100ml sữa dừa\",\"Hạt bí rang, dầu olive\",\"Muối, tiêu, nhục đậu khấu\"]", Instructions="[\"Bí đỏ gọt vỏ, cắt miếng; hành tây thái nhỏ\",\"Phi hành tây và tỏi với dầu olive đến mềm\",\"Cho bí đỏ vào, đảo đều\",\"Đổ nước dùng vào, nấu 15 phút đến bí mềm\",\"Dùng máy xay sinh tố xay nhuyễn\",\"Cho sữa dừa vào, đun ấm lại, nêm muối tiêu\",\"Rắc hạt bí rang và vài giọt dầu olive lên trên\"]" },
            new Recipe { Id=14, Name="Cá basa hấp gừng hành", Calories=220, Protein=32.0, Carb=4.0,  Fat=8.0,  Fiber=1.0,  Category="Dinner",    Tags="[\"vietnamese\",\"lean\",\"steamed\"]",IsActive=true, PrepTimeMin=20, ImageUrl="https://images.unsplash.com/photo-1519708227418-c8fd9a32b7a2?w=400",   Description="Cá basa hấp gừng hành thanh đạm, giữ trọn dưỡng chất, phù hợp ăn kiêng.", Ingredients="[\"300g cá basa phi lê\",\"1 nhánh gừng\",\"3 nhánh hành lá\",\"2 muỗng nước tương\",\"1 muỗng dầu mè\",\"1 muỗng dầu ăn\",\"Tiêu xay\"]", Instructions="[\"Cá basa rửa sạch, thấm khô, xếp vào đĩa hấp\",\"Gừng thái chỉ, rải đều lên cá\",\"Hấp cá trong 12-15 phút đến chín\",\"Đun nóng dầu ăn đến khói\",\"Lấy cá ra, rắc hành lá và tiêu\",\"Rưới nước tương và dầu mè lên cá\",\"Đổ dầu nóng lên để dậy mùi thơm\"]" },
            new Recipe { Id=15, Name="Salad dưa chuột thanh mát",Calories=95, Protein=3.0, Carb=12.0, Fat=4.0,  Fiber=2.0,  Category="Snack",     Tags="[\"vegan\",\"low-cal\",\"refreshing\"]",IsActive=true, PrepTimeMin=10, ImageUrl="https://images.unsplash.com/photo-1540420773420-3366772f4999?w=400",  Description="Salad dưa chuột giòn mát, ít calo, phù hợp ăn vặt lành mạnh bất cứ lúc nào.", Ingredients="[\"2 quả dưa chuột\",\"1/2 quả hành tây tím\",\"Vài cọng rau mùi\",\"2 muỗng giấm gạo\",\"1 muỗng dầu mè\",\"1 muỗng nước mắm\",\"Ớt, tỏi băm\",\"Mè rang\"]", Instructions="[\"Dưa chuột thái lát mỏng hoặc gọt sọc rồi thái\",\"Hành tây tím thái lát mỏng, ngâm nước đá 5 phút bớt hăng\",\"Pha nước trộn: giấm + dầu mè + nước mắm + tỏi + ớt\",\"Trộn dưa chuột và hành tây với nước trộn\",\"Để tủ lạnh 15 phút cho ngấm\",\"Rắc mè rang và rau mùi trước khi ăn\"]" },
            new Recipe { Id=16, Name="Thịt heo nạc kho tiêu",  Calories=310, Protein=36.0, Carb=6.0,  Fat=15.0, Fiber=0.5,  Category="Dinner",    Tags="[\"vietnamese\",\"high-protein\"]",   IsActive=true, PrepTimeMin=35, ImageUrl="https://images.unsplash.com/photo-1559847844-5315695dadae?w=400",   Description="Thịt heo nạc kho tiêu đậm đà, ăn với cơm gạo lứt hoặc cơm cauli.", Ingredients="[\"300g thịt heo nạc vai\",\"2 muỗng nước mắm\",\"1 muỗng nước tương\",\"1 muỗng đường dừa\",\"1 muỗng tiêu xay\",\"3 tép tỏi\",\"2 muỗng nước dừa\",\"Hành lá\"]", Instructions="[\"Thịt heo thái miếng 2cm, rửa sạch thấm khô\",\"Ướp với nước mắm, nước tương, đường, tiêu, tỏi 20 phút\",\"Cho thịt vào nồi, đảo đều trên lửa vừa\",\"Thêm nước dừa, kho liu riu 20-25 phút\",\"Nêm lại gia vị, kho đến khi nước cạn sệt\",\"Rắc hành lá và tiêu thêm trước khi dọn\"]" }
        );
    }
}
