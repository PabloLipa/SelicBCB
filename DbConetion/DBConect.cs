using Microsoft.Data.Sqlite;
using SelicBCB___Pablo_Lipa.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelicBCB___Pablo_Lipa.DbConetion
{
    public class DBConect
    {
        private const string DbFileName = "selic_data.db";
        private readonly string _connectionString;

        public DBConect()
        {
            string dbPath = Path.Combine("db", DbFileName);
            Directory.CreateDirectory("db");
            _connectionString = $"Data Source={dbPath};";
            CreateTableIfNotExists();
        }

        private void CreateTableIfNotExists()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Selic (
                    Data TEXT PRIMARY KEY,
                    Valor REAL NOT NULL
                );";
            cmd.ExecuteNonQuery();
        }

        public async Task<List<SelicBC>> GetSavedSelicDataAsync()
        {
            var result = new List<SelicBC>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Data, Valor FROM Selic ORDER BY Data ASC";

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new SelicBC
                {
                    data = reader.GetString(0),
                    valor = reader.GetFloat(1)
                });
            }

            return result;
        }


        public void ClearSelicData()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Selic";
            cmd.ExecuteNonQuery();
        }
    }
}
