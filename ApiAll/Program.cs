using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace ApiAll
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //NewtonsoftJson
            var PathWithFolderName = Path.Combine(Directory.GetCurrentDirectory(), "images");
            

            try
            {
                if (!Directory.Exists(PathWithFolderName))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(PathWithFolderName);

                }
            }
            catch (Exception) { 
            
            }


            var firstDayOfMonth = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            Console.WriteLine(firstDayOfMonth.ToString());
            Console.WriteLine(lastDayOfMonth.AddHours(23.9999).ToString());

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(opts =>
                    {
                        // HTTP portlar (Linux server uchun)
                        // IPAddress.Loopback o'chirilgan (faqat localhost da ishlaydi, tashqi IP dan ulanish mumkin emas)
                        // opts.Listen(IPAddress.Loopback, port: 5002);
                        opts.ListenAnyIP(5009); // Barcha IP larda ishlaydi (tashqi IP dan ham ulanish mumkin)
                        // HTTPS portlar o'chirilgan (sertifikat yo'q bo'lsa)
                        // opts.ListenLocalhost(5004, opts => opts.UseHttps());
                        // opts.ListenLocalhost(5005, opts => opts.UseHttps());
                        // opts.ListenAnyIP(5006, opts => opts.UseHttps());
                    });
                });
    }
}
