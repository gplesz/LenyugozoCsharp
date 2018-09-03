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
using Serilog;

namespace bot.server
{
    public class Program
    {
        public static void Main(string[] args)
        {

            try
            {
                //Ez a nem kezelt kivételeket adja át,
                //azonban amit keresünk, az egy másik thread kivételkezelésbe nem került kivétele,
                //így nem számít nem kezelt kivételnek, ezért MOST nem segít
                AppDomain.CurrentDomain.UnhandledException+=LogUnhandledException;

                //ez a megoldás valamennyi multi-thread folyxamat kivételeit naplózza
                //figyelem, ez az esemény akkor is kiváltódik, ha a kivétel kezelt kivétel
                AppDomain.CurrentDomain.FirstChanceException+=LogFirstChanceException;
                
                //hívás konfigurálja a naplót
                var host = CreateWebHostBuilder(args).Build();
                
                //innentől kezdve él a naplózásunk
                Serilog.Log.Information("app started"); 

                host.Run();
            }
            catch (System.Exception ex)
            {
                Serilog.Log.Fatal(ex, "host terminated unexpectedly");
            }
            finally
            {
                Serilog.Log.Information("app stopped");
                Serilog.Log.CloseAndFlush();
            }
        }

        private static void LogFirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            Serilog.Log.Error(e.Exception, "FirstChanceException raised");
        }

        private static void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Serilog.Log.Error((Exception)e.ExceptionObject, "Unhandled exception");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                //Ahhoz, hogy az ASP.NET alkalmazásunk belsó naplójához is hozzáférjünk
                //kell ez a csomag: dotnet add package Serilog.AspNetCore
                //és ez a beállítás
                .UseSerilog()
                ;
    }
}
