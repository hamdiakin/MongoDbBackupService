@echo off
setlocal

:: Parameters
set mongoDumpPath=%1
set mongoDbAddress=%2
set databaseName=%3
set backupDir=%4
set logDir=%5
set authPart=%6

:: Display parameters for debugging
echo mongoDumpPath: %mongoDumpPath%
echo mongoDbAddress: %mongoDbAddress%
echo databaseName: %databaseName%
echo backupDir: %backupDir%
echo logDir: %logDir%

:: Check if mongodump path exists
if not exist "%mongoDumpPath%" (
    echo mongodump path does not exist: %mongoDumpPath%
    exit /b 267
)

:: Create backup directory if it does not exist
if not exist "%backupDir%" (
    echo Creating backup directory: %backupDir%
    mkdir "%backupDir%"
)

:: Create log directory if it does not exist
if not exist "%logDir%" (
    echo Creating log directory: %logDir%
    mkdir "%logDir%"
)

:: Log the constructed command for debugging
echo "%mongoDumpPath%" --uri="%mongoDbAddress%" --db=%databaseName% --gzip --out="%backupDir%" %authPart% > "%logDir%\full-backup-command.log"

:: Run the backup command and capture output
"%mongoDumpPath%" --uri="%mongoDbAddress%" --db=%databaseName% --gzip --out="%backupDir%" %authPart% > "%logDir%\full-backup.log" 2>&1

:: Check the exit code of the backup command
if %ERRORLEVEL% neq 0 (
    echo Backup failed with exit code %ERRORLEVEL%
    exit /b %ERRORLEVEL%
)

:: Write oplog timestamp to a file
echo %date% %time% > "%backupDir%\oplog_since.txt"

echo Backup completed successfully
exit /b 0
