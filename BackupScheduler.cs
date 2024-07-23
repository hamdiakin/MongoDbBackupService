using MongoDbBackupService;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MongoBackupService
{
    public class BackupScheduler
    {
        private readonly IBackupService _backupService;
        private static Timer _timer;

        public BackupScheduler(IBackupService backupService)
        {
            _backupService = backupService;
        }

        public void Start()
        {
            // Set up the timer for scheduled backups
            _timer = new Timer(5000); // 24 hours in milliseconds
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            // Run an initial backup immediately
            OnTimedEvent(null, null);
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                _backupService.PerformBackup(BackupType.Full);
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
