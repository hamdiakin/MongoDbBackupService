using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDbBackupService;

namespace MongoBackupService
{
    class Program
    {
        static void Main(string[] args)
        {
            // Build and run the host
            var host = CreateHostBuilder(args).Build();

            // Start the backup scheduler
            var backupScheduler = host.Services.GetRequiredService<BackupScheduler>();
            backupScheduler.Start();

            // Keep the console open for debugging
            Console.WriteLine("Backup service is running. Press any key to exit...");
            Console.ReadKey();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IBackupService, BackupService>();
                    services.AddSingleton<BackupScheduler>();
                });
    }
}
