using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using MongoDbBackupService;
using NLog;

namespace MongoBackupService
{
    public class BackupService : IBackupService
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration configuration;
        private readonly string mongoDumpPath;
        private readonly string mongoDbAddress;
        private readonly string mongoDbUsername;
        private readonly string mongoDbPassword;
        private readonly string backupDir;
        private readonly string fullBackupScript;
        private readonly string incrementalBackupScript;
        private readonly string differentialBackupScript;
        private readonly string databaseName;

        public BackupService(IConfiguration configuration)
        {
            this.configuration = configuration;
            mongoDumpPath = configuration["MongoDumpPath"]; // Path to mongodump.exe
            mongoDbAddress = configuration["MongoDb:Address"]; // MongoDB server address
            mongoDbUsername = configuration["MongoDb:Username"]; // Optional
            mongoDbPassword = configuration["MongoDb:Password"]; // Optional
            backupDir = configuration["BackupDirectory"];
            fullBackupScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupScripts", "FullBackup.cmd");
            incrementalBackupScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupScripts", "IncrementalBackup.cmd");
            differentialBackupScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupScripts", "DifferentialBackup.cmd");
            databaseName = configuration["MongoDb:DatabaseName"];
        }

        public void PerformBackup(BackupType backupType)
        {
            try
            {
                // Prepare directories
                string backupTypeDir = Path.Combine(backupDir, backupType.ToString().ToLower());
                string logDir = Path.Combine(backupTypeDir, "log");
                Directory.CreateDirectory(backupTypeDir);
                Directory.CreateDirectory(logDir);

                // Prepare script and arguments
                string scriptPath = backupType switch
                {
                    BackupType.Full => fullBackupScript,
                    BackupType.Incremental => incrementalBackupScript,
                    BackupType.Differential => differentialBackupScript,
                    _ => throw new ArgumentOutOfRangeException(nameof(backupType), backupType, null)
                };

                string timestamp = DateTime.Now.ToString("dd.MM.yyyy-HH.mm.ss");
                string authPart = !string.IsNullOrEmpty(mongoDbUsername) && !string.IsNullOrEmpty(mongoDbPassword)
                    ? $"--username {mongoDbUsername} --password {mongoDbPassword} "
                    : string.Empty;

                string arguments = $"\"{mongoDumpPath}\" --uri=\"{mongoDbAddress}\" --archive=\"{backupTypeDir}\\{backupType.ToString().ToLower()}_backup_{timestamp}.gz\" --gzip --verbose {authPart}--db {databaseName}";

                // Configure the process start info
                ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe", $"/c {arguments}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = processStartInfo })
                {
                    process.OutputDataReceived += (sender, args) => { if (args.Data != null) logger.Info(args.Data); };
                    process.ErrorDataReceived += (sender, args) => { if (args.Data != null) logger.Error(args.Data); };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"An error occurred while performing {backupType} backup.");
            }
        }
    }
}
