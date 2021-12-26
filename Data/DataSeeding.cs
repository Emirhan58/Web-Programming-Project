using WebProgrammingProject.Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebProgrammingProject.Models;
using WebProgrammingProject.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebProgrammingProject.Data
{
    public class DataSeeding
    {
        private readonly ApplicationIdentityDbContext _context;

        public DataSeeding(ApplicationIdentityDbContext context)
        {
            _context = context;
        }

        public async void SeedAdminUserAndRoles()
        {
            var user = new ApplicationUser
            {
                UserName = "g201210010@ogr.sakarya.edu.tr",
                NormalizedUserName = "G201210010@OGR.SAKARYA.EDU.TR",
                Email = "g201210010@ogr.sakarya.edu.tr",
                NormalizedEmail = "G201210010@OGR.SAKARYA.EDU.TR",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };


            var roleStore = new RoleStore<IdentityRole>(_context);

            string[] roles = new string[] { "Owner", "Admin", "User" };

            foreach (string role in roles)
            {
                if(!_context.Roles.Any(r => r.Name == role))
                    await roleStore.CreateAsync(new IdentityRole { Name = role, NormalizedName = role.ToUpper() });
            }


            if (!_context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, "123");
                user.PasswordHash = hashed;
                var userStore = new UserStore<ApplicationUser>(_context);
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, "Owner");
                await userStore.AddToRoleAsync(user, "Admin");
            }

            await _context.SaveChangesAsync();
        }

        public static void Seed(IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<ApplicationIdentityDbContext>();
            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            context.Database.Migrate();

            var categories = new List<Category>() {
                new Category() {Name="Macera"},
                new Category() {Name="Romantik"},
                new Category() {Name="Bilim Kurgu"},
                new Category() {Name="Savaş"},
                new Category() {Name="Komedi"},
                new Category() {Name="Polisiye"},
                new Category() {Name="Hiciv"},
                new Category() {Name="Psikolojik Kurgu"},
                new Category() {Name="Roman"},
                new Category() {Name="Edebi Kurgu"}
            };

            var products = new List<Product>() {
                            new Product
                            {
                                Name = "Suç ve Ceza",
                                ShortDescription = "Rus yazar Fyodor Dostoyevski tarafından yazılan romandır. Dostoyevski'nin beş yıl süren Sibirya sürgününün dönüşü yazdığı tam uzunluktaki ikinci romanıdır. Yazarın olgunluk döneminin ilk büyük romanı olarak kabul edilir.",
                                Description = "Suç ve Ceza; parası için bir tefeci kadını öldürmeyi tasarlayan, Saint Petersburg'da yaşayan fakir bir öğrenci olan Rodion Romanoviç Raskolnikov'un, manevi ıstırabı ve ahlaki ikilemlerine odaklanır. Öldürmeden önce, Raskolnikov parayla kendini yoksulluktan kurtarabileceğine ve büyük işler yapmaya devam edeceğine inanır fakat karışıklık, tereddüt ve şans, ahlaki olarak haklı bir öldürme planını bulanıklaştırır.",
                                ImageAdress = "Suc ve ceza.jpg",
                                Author = "Fyodor Dostoyevski",
                                Categories = new List<Category>() {categories[5],categories[7],categories[2]}
                            },
                            new Product
                            {
                                Name = "Hayvan Çiftliği",
                                ShortDescription = "George Orwell'in mecazi bir dille yazılmış, fabl tarzındaki siyasi hiciv romanı.",
                                Description = "Roman, Stalinizmin eleştirisidir. Kendisini her türlü totalitarizme karşı bir demokratik sosyalist olarak tanımlayan Orwell bu romanında SSCB'nin kuruluşundan itibaren meydana gelen önemli olayları kara mizah yoluyla ve mecazi bir dille anlatır. Hayvan Çiftliği çok yankı uyandırmış ve olumlu eleştiriler almıştır. Bir Stalinizm eleştirisi olmakla birlikte, II.Dünya Savaşı yıllarında müttefiklerini kızdırmak istemeyen Birleşik Krallık'ta sansüre uğramıştır. Romanın çizgi filmi çekilirken CIA tarafından değiştirildiği iddia edilmektedir. Roman 1999'da bu kez konusuna daha sadık bir senaryoyla filme çekilmiştir.",
                                ImageAdress = "Hayvan ciftligi.jpg",
                                Author = "George Orwell",
                                Categories = new List<Category>() {categories[6],categories[8]}
                            },
                            new Product
                            {
                                Name = "Sineklerin Tanrısı",
                                ShortDescription = " Nobel Edebiyat Ödüllü İngiliz romancı ve şair William Golding'in 1954 yılında yazdığı alegorik romanıdır.",
                                Description = "Sineklerin Tanrısı, Liderlik savaşının insanların doğal yapısında olduğunu ve bunu kazanmak için de dost kazanma ve düşman kaybetme (gerekirse yok etme) yöntemlerini uygulamasını gösteren bir roman. Gruplaşmaların temelinde insanın en derinlerinde saklı pırıltıları ve kötülükleri meydana çıkarma uğraşındaki insanları betimliyor.",
                                ImageAdress = "Sineklerin tanrisi.jpg",
                                Author = "William Golding",
                                Categories = new List<Category>() {categories[8],categories[0],categories[7]}
                            },
                            new Product
                            {
                                Name = "Kızıl Veba",
                                ShortDescription = "Jack London bu romanında uygarlığın kendisini nasıl bir sona getirdiğini (kızıl veba) akılcı bir şekilde dile getirmiştir.",
                                Description = "İnsanlığın sona yaklaşmasına neden olan 'kızıl veba' değil yine insanın daha çok uygarlaşma çabasıdır. Kızıl vebadan sonra dünyada binin altında hayatta kalanlar uygarlığa doğru çoğalarak gidecek ve yeniden alaşağı olacaktır. bu kısırdöngü böyle devam edecektir ve bu noktada Jack London Şunu sorar 'Bu nereye kadar devam edecektir, bunun anlamı nedir, ne diye vardır bu kısır döngü'. Roman insanlığın ilkel ve vahşi yaşamına geri dönüş olarak görülebilir-sadece güçlünün ayakta kalabileceği bir dünya.",
                                ImageAdress = "Kizil veba.jpg",
                                Author = "Jack London",
                                Categories = new List<Category>() {categories[3],categories[7],categories[2]}
                            },
                            new Product
                            {
                                Name = "Bilinmeyen Bir Kadının Mektubu",
                                ShortDescription = " ilk kez 1922'de yayınlanan, Stefan Zweig tarafından yazılan uzun öykü. Mektup biçiminde yazılan eser, ünlü bir yazarın hatırlamadığı bir kadından aldığı mektuptan oluşuyor.",
                                Description = "Öykü, roman yazarı R. olarak anılan yazarın tatilden döndükten sonra, aldığı imzasız mektubu okumasıyla başlıyor. Mektup yazarın hiç tanımadığı bir kadından gelmiştir ve Sana,beni hiç tanımamış olan sana, diye başlar. Kadın küçükken Viyana'da yazarla aynı apartmanda yaşamış, daha sonra annesiyle birlikte Innsbruck'a taşındığı zaman bile bu adama karşı tutkusu hiç eksilmemiş ve 18 yaşına geldiği zaman tekrar Viyana'ya dönmüştür. Yazarla yeniden görüşmeye çalışan kadın üç gecelik beraberliğin ardından unutulmuş olarak hayatına devam eder.",
                                ImageAdress = "Bilinmeyen bir kadinin mektubu.png",
                                Author = "Stefan Zweig",
                                Categories = new List<Category>() {categories[8],categories[1]}
                            }
            };
            
            
            
            
            

            if (!context.Database.GetPendingMigrations().Any())
            {
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(categories);
                }
                if (!context.Products.Any())
                {
                    context.Products.AddRange(products);
                }
                context.SaveChanges();
            }
        }
    }
}
