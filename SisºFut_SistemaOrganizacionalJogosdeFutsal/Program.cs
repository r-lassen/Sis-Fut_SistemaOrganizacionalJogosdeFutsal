using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });

        public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder
                .UseUrls("http://0.0.0.0:5000") // ou "http://192.168.18.218:5000"
                .UseStartup<Startup>();
        });



    }
}
