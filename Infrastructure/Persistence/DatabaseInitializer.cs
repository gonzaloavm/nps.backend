using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Dapper;
using System.Text.RegularExpressions;

namespace Infrastructure.Persistence
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;
        private readonly string _masterConnectionString;

        public DatabaseInitializer(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");

            // Construir cadena para conectar a master (misma autenticación, diferente base de datos)
            var builder = new SqlConnectionStringBuilder(_connectionString);
            builder.InitialCatalog = "master";
            _masterConnectionString = builder.ConnectionString;
        }

        public async Task InitializeAsync()
        {
            // Asegurar que la base de datos existe
            if(!await EnsureDatabaseExistsAsync())
            {
                // Ejecutar script de creación de tablas y datos iniciales solo si no existe
                await ExecuteScriptAsync();
            }

            
        }

        private async Task<bool> EnsureDatabaseExistsAsync()
        {
            var databaseName = new SqlConnectionStringBuilder(_connectionString).InitialCatalog;

            using var connection = new SqlConnection(_masterConnectionString);
            var exists = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM sys.databases WHERE name = @name",
                new { name = databaseName });

            if (exists == 0)
            {
                Console.WriteLine($"Creando base de datos '{databaseName}'...");
                await connection.ExecuteAsync($"CREATE DATABASE [{databaseName}]");
                Console.WriteLine("Base de datos creada.");
                return false; // No existia
            }
            else
            {
                return true; // Si existe
            }
        }

        private async Task ExecuteScriptAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Infrastructure.Persistence.Scripts.InitDatabase.sql";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                Console.WriteLine("Script de inicialización no encontrado.");
                return;
            }

            using var reader = new StreamReader(stream);
            var sql = await reader.ReadToEndAsync();

            // Dividir por GO si es necesario
            var commands = sql.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

            using var connection = new SqlConnection(_connectionString);
            foreach (var command in commands)
            {
                if (!string.IsNullOrWhiteSpace(command))
                {
                    await connection.ExecuteAsync(command);
                }
            }

            Console.WriteLine("Base de datos inicializada correctamente.");
        }
    }
}
