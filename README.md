# MongoDB Backup Service

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [How It Works](#how-it-works)
- [Logging](#logging)
- [To-Do](#to-do)
- [Contribution](#contribution)
- [License](#license)
- [Contact](#contact)

## Overview

The MongoDB Backup Service is a C# console application designed to create backups of MongoDB databases. It uses MongoDB's `mongodump` tool and batch scripts to manage the backup process.

## Features

- **Full Backup:** Create a full backup of a MongoDB database.
- **Scheduled Backups:** Automate backup creation using a scheduled task.

## Requirements

- [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0) or later
- [MongoDB Tools](https://www.mongodb.com/try/download/database-tools) (specifically `mongodump`)
- [NLog](https://nlog-project.org/) for logging

## Installation

1. **Clone the Repository**

    ```bash
    git clone https://github.com/yourusername/mongodb-backup-application.git
    cd mongodb-backup-application
    ```

2. **Install Dependencies**

    Restore NuGet packages with:

    ```bash
    dotnet restore
    ```

3. **Configure the Application**

    Create an `appsettings.json` file in the project root directory with the following structure:

    ```json
    {
      "MongoDb": {
        "Address": "mongodb://localhost:27017",
        "Username": "",
        "Password": "",
        "DatabaseName": "trial"
      },
      "MongoDumpPath": "MongoExecutables\\mongodump.exe",
      "BackupDirectory": "C:\\Users\\yourusername\\Desktop\\MongoDBBackupProject\\Backups"
    }
    ```

    Adjust paths and settings according to your environment.

## Usage

1. **Build the Application**

    ```bash
    dotnet build
    ```

2. **Run the Application**

    ```bash
    dotnet run
    ```

    The application starts a backup scheduler and performs backups based on the configured schedule.

## How It Works

1. **Configuration Loading:** The application loads configuration settings from `appsettings.json` using `IConfiguration`.

2. **Backup Operation:**
   - The `BackupService` class determines the correct script to execute for backups.
   - Constructs arguments for the `mongodump` command.
   - Starts the process and logs output and errors using NLog.

3. **Backup Scheduler:**
   - The `BackupScheduler` class uses a timer to schedule regular backups.
   - It initiates a full backup at defined intervals.

## Logging

The application utilizes [NLog](https://nlog-project.org/) for logging. Logs are directed to the console but can be configured to write to files or other targets as needed.

## To-Do

- **Error Handling Enhancements:** Improve error handling for various edge cases.
- **Configuration Validation:** Implement validation to ensure configuration settings are correct.
- **Script Customization:** Allow batch script customization via configuration.
- **UI Enhancements:** Consider creating a graphical or more interactive command-line interface.

## Contribution

Contributions are welcome! Please submit pull requests or open issues to improve the project. Follow the [GitHub contribution guidelines](https://docs.github.com/en/github/collaborating-with-issues-and-pull-requests) for details.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For questions or support, please open an issue on the [GitHub repository](https://github.com/yourusername/mongodb-backup-application) or contact [your-email@example.com](mailto:your-email@example.com).




# MongoDB Restore Application
## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [How It Works](#how-it-works)
- [Logging](#logging)
- [To-Do](#to-do)
- [Contribution](#contribution)
- [License](#license)
- [Contact](#contact)

## Overview

The MongoDB Restore Application is a C# console application designed to restore MongoDB databases from backups. Leveraging MongoDB's `mongorestore` tool and batch scripts, this application offers a streamlined approach to database restoration.

## Features

- **Restore All Data:** Restore an entire database from a backup.
- **Restore Specific Collection:** Restore a particular collection from a backup.
- **Restore to Different Database:** Restore data to a different database from the original backup.

## Requirements

- [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0) or later
- [MongoDB Tools](https://www.mongodb.com/try/download/database-tools) (specifically `mongorestore`)
- [NLog](https://nlog-project.org/) for logging

## Installation

1. **Clone the Repository**

    ```bash
    git clone https://github.com/yourusername/mongodb-restore-application.git
    cd mongodb-restore-application
    ```

2. **Install Dependencies**

    Restore NuGet packages with:

    ```bash
    dotnet restore
    ```

3. **Configure the Application**

    Create an `appsettings.Restore.json` file in the project root directory with the following structure:

    ```json
    {
      "MongoDb": {
        "Address": "mongodb://localhost:27017",
        "Username": "",
        "Password": "",
        "DatabaseName": "trial"
      },
      "MongoRestorePath": "MongoExecutables\\mongorestore.exe",
      "RestoreDirectory": "C:\\Users\\yourusername\\Desktop\\MongoDBBackupProject\\Restores",
      "RestoreType": "ToDifferentDatabase",
      "RestoreSource": "C:\\Users\\yourusername\\Desktop\\MongoDBBackupProject\\Backups\\full\\2024-07-24-10-00-18\\trial",
      "CollectionName": "b",
      "TargetDatabaseName": "trial-updated"
    }
    ```

    Adjust paths and settings according to your environment.

## Usage

1. **Build the Application**

    ```bash
    dotnet build
    ```

2. **Run the Application**

    ```bash
    dotnet run
    ```

    The application reads the configuration from `appsettings.Restore.json` and performs the restore operation based on the specified settings.

## How It Works

1. **Configuration Loading:** The application loads configuration settings from `appsettings.Restore.json` using `IConfiguration`.

2. **Restore Operation:**
   - The `RestoreService` class selects the appropriate script based on the `RestoreType` configuration.
   - Constructs arguments for the `mongorestore` command.
   - Starts the process and logs output and errors using NLog.

3. **Restore Types:**
   - **AllData:** Restores the entire database.
   - **SpecificCollection:** Restores a specific collection.
   - **ToDifferentDatabase:** Restores data to a different database.

## Logging

The application utilizes [NLog](https://nlog-project.org/) for logging. Logs are directed to the console but can be configured to write to files or other targets as needed.

## To-Do

- **Error Handling Enhancements:** Improve error handling for various edge cases.
- **Configuration Validation:** Implement validation to ensure configuration settings are correct.
- **Script Customization:** Allow batch script customization via configuration.
- **UI Enhancements:** Consider creating a graphical or more interactive command-line interface.

## Contribution

Contributions are welcome! Please submit pull requests or open issues to improve the project. Follow the [GitHub contribution guidelines](https://docs.github.com/en/github/collaborating-with-issues-and-pull-requests) for details.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For questions or support, please open an issue on the [GitHub repository](https://github.com/yourusername/mongodb-restore-application) or contact [your-email@example.com](mailto:your-email@example.com).
