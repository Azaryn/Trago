using Npgsql;

namespace TraGo.Models
{
    public sealed class Database
    {
        private static readonly Lazy<Database> _instance = new(() => new Database());
        public static Database Instance => _instance.Value;

    private readonly string _connectionString =
        "Host=localhost;Port=5432;Username=postgres;Password=evie789;Database=trago_db";

        private Database() { }

        public NpgsqlConnection GetConnection()
        {
            var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}