using Npgsql;

namespace TraGo.Models
{
    public class UserModel
    {
        private readonly Database _db = Database.Instance;

        public User? Authenticate(string username, string password)
        {
            using var conn = _db.GetConnection();
            string sql = @"
                SELECT id, username, password, full_name, role::text, nik, station_code
                FROM users
                WHERE LOWER(username) = LOWER(@username) AND password = @password;";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("password", password);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return MapReaderToUser(reader);
        }

        public User? GetUserById(int userId)
        {
            using var conn = _db.GetConnection();
            string sql = @"
                SELECT id, username, password, full_name, role::text, nik, station_code
                FROM users
                WHERE id = @id;";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", userId);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return MapReaderToUser(reader);
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using var conn = _db.GetConnection();
            string sql = @"
                SELECT id, username, password, full_name, role::text, nik, station_code
                FROM users
                ORDER BY id ASC;";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var user = MapReaderToUser(reader);
                if (user != null) users.Add(user);
            }
            return users;
        }

        private static User? MapReaderToUser(NpgsqlDataReader reader)
        {
            int id = reader.GetInt32(0);
            string uname = reader.GetString(1);
            string pass = reader.GetString(2);
            string name = reader.GetString(3);
            string roleStr = reader.GetString(4);
            string nik = reader.IsDBNull(5) ? "" : reader.GetString(5);
            string station = reader.IsDBNull(6) ? "" : reader.GetString(6);

            if (!Enum.TryParse<UserRole>(roleStr, ignoreCase: true, out var role))
                return null;

            string extra = role == UserRole.Customer ? nik : station;
            return UserFactory.CreateUser(role, uname, pass, name, extra, id);
        }

        public bool RegisterCustomer(Customer customer)
        {
            try
            {
                using var conn = _db.GetConnection();
                string sql = @"
                    INSERT INTO users (username, password, full_name, role, nik)
                    VALUES (@username, @password, @full_name, @role::user_role, @nik);";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("username", customer.Username);
                cmd.Parameters.AddWithValue("password", customer.Password);
                cmd.Parameters.AddWithValue("full_name", customer.FullName);
                cmd.Parameters.AddWithValue("role", customer.Role.ToString());
                cmd.Parameters.AddWithValue("nik", customer.Nik);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (PostgresException ex) when (ex.SqlState == "23505")
            {
                return false;
            }
        }

        public bool CreateGateOfficer(GateOfficer officer)
        {
            try
            {
                using var conn = _db.GetConnection();
                string sql = @"
                    INSERT INTO users (username, password, full_name, role, station_code)
                    VALUES (@u, @p, @n, @r::user_role, @s);";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("u", officer.Username);
                cmd.Parameters.AddWithValue("p", officer.Password);
                cmd.Parameters.AddWithValue("n", officer.FullName);
                cmd.Parameters.AddWithValue("r", officer.Role.ToString());
                cmd.Parameters.AddWithValue("s", officer.StationCode);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (PostgresException ex) when (ex.SqlState == "23505")
            {
                return false;
            }
        }

        public bool UpdateFullName(int userId, string newName)
        {
            using var conn = _db.GetConnection();
            using var cmd = new NpgsqlCommand("UPDATE users SET full_name = @n WHERE id = @id;", conn);
            cmd.Parameters.AddWithValue("n", newName);
            cmd.Parameters.AddWithValue("id", userId);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool UpdatePassword(int userId, string newPassword)
        {
            using var conn = _db.GetConnection();
            using var cmd = new NpgsqlCommand("UPDATE users SET password = @p WHERE id = @id;", conn);
            cmd.Parameters.AddWithValue("p", newPassword);
            cmd.Parameters.AddWithValue("id", userId);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteUser(int userId)
        {
            using var conn = _db.GetConnection();
            using var cmd = new NpgsqlCommand("DELETE FROM users WHERE id = @id;", conn);
            cmd.Parameters.AddWithValue("id", userId);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}