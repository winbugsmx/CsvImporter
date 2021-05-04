using CsvImporter.Dao;
using CsvImporter.Managers.CvsManager;
using CsvImporter.Managers.DatabaseManager;
using CsvImporter.Managers.FileManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsvImporter
{
    public class Startup
    {
        private static IConfigurationRoot configuration;
        private static IServiceProvider provider;
        public IServiceProvider Provider => provider;
        public IConfiguration Configuration => configuration;

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            configuration = builder.Build();

            var services = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<IStorageManager, AzureStorageManager>()
                .AddSingleton<ICvsManager, ReadCsvManager>()
                .AddSingleton<IDatabaseManager, DatabaseManager>()
                .AddSingleton<ISqlDao, SqlDao>();

            provider = services.BuildServiceProvider();
        }
    }
}