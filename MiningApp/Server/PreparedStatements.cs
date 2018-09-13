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

        public class GetUserClients
        {
            public static MySqlCommand GetCommand(string userID)
            {
                string sql = "SELECT * FROM Clients WHERE UserID = @userID";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "@userID", userID);

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
                    $"{ColumnnNames.Users.LastLogin}, {ColumnnNames.Users.RequiresLogin}) VALUES (?userID, ?email, ?password, ?created, ?lastlogin, ?requiresLogin) " +
                    $"ON DUPLICATE KEY UPDATE {ColumnnNames.Users.Email}=?email, {ColumnnNames.Users.LastLogin}=?lastlogin, {ColumnnNames.Users.RequiresLogin}=?requiresLogin";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "?userID", user.ID);
                AddParameter(cmd, "?password", user.Password);
                AddParameter(cmd, "?email", user.Email);
                AddParameter(cmd, "?created", DateTime.Now);
                AddParameter(cmd, "?lastlogin", DateTime.Now);
                AddParameter(cmd, "?requiresLogin", user.RequiresLogin);

                return cmd;
            }
        }

        public class InsertClientMessage
        {
            public static MySqlCommand GetCommand(ClientMessageModel message)
            {
                string sql = $"INSERT INTO ClientMessages ({ColumnnNames.ClientMessages.ClientID}, {ColumnnNames.ClientMessages.Timestamp}," +
                    $"{ColumnnNames.ClientMessages.Message}, {ColumnnNames.ClientMessages.Action}) VALUES (?clientID, ?timestamp, ?message, ?action)";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "?clientID", message.ClientID);
                AddParameter(cmd, "?timestamp", message.Timestamp);
                AddParameter(cmd, "?message", message.Message);
                AddParameter(cmd, "?action", message.Action.Value);

                return cmd;
            }
        }

        static void AddParameter(MySqlCommand cmd, string key, object value)
        {
            cmd.Parameters.Add(new MySqlParameter(key, value));
        }
    }
}
