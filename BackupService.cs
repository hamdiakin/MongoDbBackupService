using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Diagnostics;
using System.IO;

namespace MongoDbBackupService
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
        private readonly string differentialBackupScript;
        private readonly string databaseName;

        public BackupService(IConfiguration configuration)
        {
            this.configuration = configuration;
            mongoDumpPath = configuration["MongoDumpPath"];
            mongoDbAddress = configuration["MongoDb:Address"];
            mongoDbUsername = configuration["MongoDb:Username"];
            mongoDbPassword = configuration["MongoDb:Password"];
            backupDir = configuration["BackupDirectory"];
            fullBackupScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupScripts", "FullBackup.cmd");
            differentialBackupScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupScripts", "DifferentialBackup.cmd");
            databaseName = configuration["MongoDb:DatabaseName"];
        }

        public void PerformBackup(BackupType backupType)
        {
            try
            {
                // Prepare directories
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                string backupTypeDir = Path.Combine(backupDir, backupType.ToString().ToLower(), timestamp);
                string logDir = Path.Combine(backupTypeDir, "log");
                Directory.CreateDirectory(backupTypeDir);
                Directory.CreateDirectory(logDir);

                // Determine the correct script path
                string scriptPath = backupType switch
                {
                    BackupType.Full => fullBackupScript,
                    BackupType.Differential => differentialBackupScript,
                    _ => throw new ArgumentOutOfRangeException(nameof(backupType), backupType, null)
                };

                string authPart = !string.IsNullOrEmpty(mongoDbUsername) && !string.IsNullOrEmpty(mongoDbPassword)
                    ? $"--username \"{mongoDbUsername}\" --password \"{mongoDbPassword}\" "
                    : string.Empty;

                // Construct the arguments to pass to the batch script
                string arguments = $"\"{mongoDumpPath}\" \"{mongoDbAddress}\" \"{databaseName}\" \"{backupTypeDir}\" \"{logDir}\" {authPart}";

                // Configure the process start info to run the batch script directly
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = scriptPath,
                    Arguments = arguments,
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

                    if (process.ExitCode != 0)
                    {
                        logger.Error($"Script execution failed with exit code {process.ExitCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"An error occurred while performing {backupType} backup.");
            }
        }
    }
}
