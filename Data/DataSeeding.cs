using BuildingFormsWeb.Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingFormsWeb.Data
{
    public static class DataSeeding
    {
        public static void Seed(IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<ApplicationIdentityDbContext>();

            context.Database.Migrate();

            var authors = new List<Author>()
            {
                new Author() {Name="Fyodor", Surname="Dostoyevski"},
                new Author() {Name="George", Surname="Orwell"},
                new Author() {Name="William", Surname="Golding"},
                new Author() {Name="Jack", Surname="London"},
                new Author() {Name="Stefan", Surname="Zweig"}
            };

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
                                Author = authors[0],
                                Categories = new List<Category>() {categories[5],categories[7],categories[2]}
                            },
                            new Product
                            {
                                Name = "Hayvan Çiftliği",
                                ShortDescription = "George Orwell'in mecazi bir dille yazılmış, fabl tarzındaki siyasi hiciv romanı.",
                                Description = "Roman, Stalinizmin eleştirisidir. Kendisini her türlü totalitarizme karşı bir demokratik sosyalist olarak tanımlayan Orwell bu romanında SSCB'nin kuruluşundan itibaren meydana gelen önemli olayları kara mizah yoluyla ve mecazi bir dille anlatır. Hayvan Çiftliği çok yankı uyandırmış ve olumlu eleştiriler almıştır. Bir Stalinizm eleştirisi olmakla birlikte, II.Dünya Savaşı yıllarında müttefiklerini kızdırmak istemeyen Birleşik Krallık'ta sansüre uğramıştır. Romanın çizgi filmi çekilirken CIA tarafından değiştirildiği iddia edilmektedir. Roman 1999'da bu kez konusuna daha sadık bir senaryoyla filme çekilmiştir.",
                                ImageAdress = "Hayvan ciftligi.jpg",
                                Author = authors[1],
                                Categories = new List<Category>() {categories[6],categories[8]}
                            },
                            new Product
                            {
                                Name = "Sineklerin Tanrısı",
                                ShortDescription = " Nobel Edebiyat Ödüllü İngiliz romancı ve şair William Golding'in 1954 yılında yazdığı alegorik romanıdır.",
                                Description = "Sineklerin Tanrısı, Liderlik savaşının insanların doğal yapısında olduğunu ve bunu kazanmak için de dost kazanma ve düşman kaybetme (gerekirse yok etme) yöntemlerini uygulamasını gösteren bir roman. Gruplaşmaların temelinde insanın en derinlerinde saklı pırıltıları ve kötülükleri meydana çıkarma uğraşındaki insanları betimliyor.",
                                ImageAdress = "Sineklerin tanrisi.jpg",
                                Author = authors[2],
                                Categories = new List<Category>() {categories[8],categories[0],categories[7]}
                            },
                            new Product
                            {
                                Name = "Kızıl Veba",
                                ShortDescription = "Jack London bu romanında uygarlığın kendisini nasıl bir sona getirdiğini (kızıl veba) akılcı bir şekilde dile getirmiştir.",
                                Description = "İnsanlığın sona yaklaşmasına neden olan 'kızıl veba' değil yine insanın daha çok uygarlaşma çabasıdır. Kızıl vebadan sonra dünyada binin altında hayatta kalanlar uygarlığa doğru çoğalarak gidecek ve yeniden alaşağı olacaktır. bu kısırdöngü böyle devam edecektir ve bu noktada Jack London Şunu sorar 'Bu nereye kadar devam edecektir, bunun anlamı nedir, ne diye vardır bu kısır döngü'. Roman insanlığın ilkel ve vahşi yaşamına geri dönüş olarak görülebilir-sadece güçlünün ayakta kalabileceği bir dünya.",
                                ImageAdress = "Kizil veba.jpg",
                                Author = authors[3],
                                Categories = new List<Category>() {categories[3],categories[7],categories[2]}
                            },
                            new Product
                            {
                                Name = "Bilinmeyen Bir Kadının Mektubu",
                                ShortDescription = " ilk kez 1922'de yayınlanan, Stefan Zweig tarafından yazılan uzun öykü. Mektup biçiminde yazılan eser, ünlü bir yazarın hatırlamadığı bir kadından aldığı mektuptan oluşuyor.",
                                Description = "Öykü, roman yazarı R. olarak anılan yazarın tatilden döndükten sonra, aldığı imzasız mektubu okumasıyla başlıyor. Mektup yazarın hiç tanımadığı bir kadından gelmiştir ve Sana,beni hiç tanımamış olan sana, diye başlar. Kadın küçükken Viyana'da yazarla aynı apartmanda yaşamış, daha sonra annesiyle birlikte Innsbruck'a taşındığı zaman bile bu adama karşı tutkusu hiç eksilmemiş ve 18 yaşına geldiği zaman tekrar Viyana'ya dönmüştür. Yazarla yeniden görüşmeye çalışan kadın üç gecelik beraberliğin ardından unutulmuş olarak hayatına devam eder.",
                                ImageAdress = "Bilinmeyen bir kadinin mektubu.png",
                                Author = authors[4],
                                Categories = new List<Category>() {categories[8],categories[1]}
                            }
            };
            
            /*
            var users = new List<User>()
            {
                new User() {Name="Halit",Surname="Aydın",Username="halitaha",Email="halitaha@gmail.com",Password="1234",ImageAdress="halitaha.jpg"},
                new User() {Name="Furkan",Surname="Liman",Username="furkanliman",Email="furkanliman@gmail.com",Password="1234",ImageAdress="furkanliman.jpg"},
                new User() {Username="g201210010@ogr.sakarya.edu.tr",Email="g201210010@ogr.sakarya.edu.tr",Password="123",ImageAdress="kurucu.jpg",Name="Emirhan",Surname="Kaya",Biography="Creator of this website.",PhoneNumber="+905530558258",PlaceOfBirth="Istanbul",IsAdmin=true},
                new User() {Username="kerimcan",Email="kerimcan@gmail.com",Password="123456789",ImageAdress="kerimcan.jpg",Name="Kerimcan",IsAdmin=true}
            };
            */
            

            if (!context.Database.GetPendingMigrations().Any())
            {
                if (!context.Authors.Any())
                {
                    context.Authors.AddRange(authors);
                }
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(categories);
                }
                if (!context.Products.Any())
                {
                    context.Products.AddRange(products);
                }
                //if (context.Users.Count() == 0)
                //{
                    // context.Users.AddRange(users);
                //}
                context.SaveChanges();
            }
        }
    }
}
