namespace MongoDbBackupService
{
    public interface IBackupService
    {
        void PerformBackup(BackupType backupType);
    }
}
