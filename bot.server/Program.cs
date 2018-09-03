using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace bot.server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Ez a nem kezelt kivételeket adja át,
            //azonban amit keresünk, az egy másik thread kivételkezelésbe nem került kivétele,
            //így nem számít nem kezelt kivételnek, ezért MOST nem segít
            AppDomain.CurrentDomain.UnhandledException+=LogUnhandledException;

            //ez a megoldás valamennyi multi-thread folyxamat kivételeit naplózza
            //figyelem, ez az esemény akkor is kiváltódik, ha a kivétel kezelt kivétel
            AppDomain.CurrentDomain.FirstChanceException+=LogFirstChanceException;
            
            CreateWebHostBuilder(args).Build().Run();
        }

        private static void LogFirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            System.Console.WriteLine($"HIBA TÖRTÉNT !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            System.Console.WriteLine(e.Exception.ToString());
            System.Console.WriteLine("HIBA TÖRTÉNT !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }

        private static void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Console.WriteLine($"HIBA TÖRTÉNT !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! (Isterminating:{e.IsTerminating})");
            System.Console.WriteLine(((Exception)e.ExceptionObject).ToString());
            System.Console.WriteLine("HIBA TÖRTÉNT !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
