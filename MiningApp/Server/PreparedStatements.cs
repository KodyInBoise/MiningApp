using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class PreparedStatements
    {
        static MySqlConnection _connection => ServerHelper.Instance.GetDatabaseConnection();

        public class UpdateClient
        {
            public static MySqlCommand GetCommand(string id, string userID, DateTime timestamp)
            {
                string sql = "INSERT INTO Clients (ClientID, UserID, LastCheckin) VALUES (?id, ?userID, ?timestamp) ON DUPLICATE KEY UPDATE UserID=?userID, LastCheckin=?timestamp";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "?id", id);
                AddParameter(cmd, "?userID", userID);
                AddParameter(cmd, "?timestamp", timestamp);

                return cmd;
            }
        }

        public class GetClient
        {
            public static MySqlCommand GetCommand(string clientID)
            {
                string sql = "SELECT * FROM Clients WHERE ClientID = @clientID";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "@clientID", clientID);

                return cmd;
            }
        }

        public class GetUser
        {
            public static MySqlCommand GetCommand(string userEmail)
            {
                string sql = $"SELECT * FROM Users WHERE {ColumnnNames.Users.Email} = @email";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "@email", userEmail);

                return cmd;
            }
        }

        public class UpdateUser
        {
            public static MySqlCommand GetCommand(UserModel user)
            {
                string sql = $"INSERT INTO Users ({ColumnnNames.Users.UserID}, {ColumnnNames.Users.Email}, {ColumnnNames.Users.Password}, {ColumnnNames.Users.Created}, " +
                    $"{ColumnnNames.Users.LastLogin}) VALUES (?userID, ?email, ?password, ?created, ?lastlogin) ON DUPLICATE KEY UPDATE {ColumnnNames.Users.Email}=?email, " +
                    $"{ColumnnNames.Users.LastLogin}=?lastlogin";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "?userID", user.ID);
                AddParameter(cmd, "?password", user.Password);
                AddParameter(cmd, "?email", user.Email);
                AddParameter(cmd, "?created", DateTime.Now);
                AddParameter(cmd, "?lastlogin", DateTime.Now);

                return cmd;
            }
        }

        static void AddParameter(MySqlCommand cmd, string key, object value)
        {
            cmd.Parameters.Add(new MySqlParameter(key, value));
        }
    }
}
