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
            public static MySqlCommand GetCommand(LocalClientModel client, string userID)
            {
                string sql = "INSERT INTO Clients (ClientID, UserID, LastCheckin, PublicIP, PrivateIP, FriendlyName) VALUES (?id, ?userID, ?timestamp, ?publicIP, ?privateIP, ?friendlyName) " +
                    "ON DUPLICATE KEY UPDATE UserID=?userID, LastCheckin=?timestamp, PublicIP=?publicIP, PrivateIP=?privateIP, FriendlyName=?friendlyName";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "?id", client.ID);
                AddParameter(cmd, "?userID", userID);
                AddParameter(cmd, "?timestamp", DateTime.Now);
                AddParameter(cmd, "?publicIP", client.PublicIP);
                AddParameter(cmd, "?privateIP", client.PrivateIP);
                AddParameter(cmd, "?friendlyName", client.GetDisplayName());

                return cmd;
            }
        }

        public class GetClient
        {
            public static MySqlCommand GetCommand(string clientID)
            {
                string sql = "SELECT * FROM Clients WHERE ClientID = ?clientID";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "?clientID", clientID);

                return cmd;
            }
        }

        public class DeleteClient
        {
            public static MySqlCommand GetCommand(string clientID)
            {
                string sql = "DELETE FROM Clients WHERE ClientID=?clientID";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "?clientID", clientID);

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
                string sql = $"SELECT * FROM Users WHERE {ColumnNames.Users.Email} = @email";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "@email", userEmail);

                return cmd;
            }
        }

        public class UpdateUser
        {
            public static MySqlCommand GetCommand(UserModel user)
            {
                string sql = $"INSERT INTO Users ({ColumnNames.Users.UserID}, {ColumnNames.Users.Email}, {ColumnNames.Users.Password}, {ColumnNames.Users.Created}, " +
                    $"{ColumnNames.Users.LastLogin}, {ColumnNames.Users.RequiresLogin}) VALUES (?userID, ?email, ?password, ?created, ?lastlogin, ?requiresLogin) " +
                    $"ON DUPLICATE KEY UPDATE {ColumnNames.Users.Email}=?email, {ColumnNames.Users.LastLogin}=?lastlogin, {ColumnNames.Users.RequiresLogin}=?requiresLogin";

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
                string sql = $"INSERT INTO ClientMessages ({ColumnNames.ClientMessages.MessageID}, {ColumnNames.ClientMessages.ClientID}, " +
                    $"{ColumnNames.ClientMessages.Timestamp}, {ColumnNames.ClientMessages.Message}, {ColumnNames.ClientMessages.Action}) " +
                    $"VALUES (?messageID, ?clientID, ?timestamp, ?message, ?action)";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "?messageID", message.MessageID);
                AddParameter(cmd, "?clientID", message.ClientID);
                AddParameter(cmd, "?timestamp", message.Timestamp);
                AddParameter(cmd, "?message", message.Message);
                AddParameter(cmd, "?action", message.Action.Value);

                return cmd;
            }
        }

        public class DeleteClientMessage
        {
            public static MySqlCommand GetCommand(ClientMessageModel message)
            {
                string sql = $"DELETE FROM ClientMessages WHERE MessageID=?messageID";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "?messageID", message.MessageID);

                return cmd;
            }
        }

        public class GetClientMessages
        {
            public static MySqlCommand GetCommand(string clientID)
            {
                string sql = "SELECT * FROM ClientMessages WHERE ClientID = @clientID";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "@clientID", clientID);

                return cmd;
            }
        }

        public class InsertClientConfig
        {
            public static MySqlCommand GetCommand(SessionConfigModel config, string clientID)
            {
                string sql = $"INSERT INTO ClientConfigs ({ColumnNames.ClientConfigs.ServerID}, {ColumnNames.ClientConfigs.ClientID}, {ColumnNames.ClientConfigs.Name}, " +
                    $"{ColumnNames.ClientConfigs.MinerName}, {ColumnNames.ClientConfigs.CryptoName}, {ColumnNames.ClientConfigs.Status}) VALUES (?serverID, ?clientID, " +
                    $"?name, ?minerName, ?cryptoName, ?status) ON DUPLICATE KEY UPDATE {ColumnNames.ClientConfigs.Name}=?name, {ColumnNames.ClientConfigs.MinerName}=?minerName, " +
                    $"{ColumnNames.ClientConfigs.CryptoName}=?cryptoName, {ColumnNames.ClientConfigs.Status}=?status";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "?serverID", config.ServerID);
                AddParameter(cmd, "?clientID", clientID);
                AddParameter(cmd, "?name", config.Name ?? "");
                AddParameter(cmd, "?minerName", config.Miner.Name ?? "");
                AddParameter(cmd, "?cryptoName", config.CryptoName ?? "");
                AddParameter(cmd, "?status", config.Session?.CurrentStatus ?? 0);

                return cmd;
            }
        }

        public class GetClientConfigs
        {
            public static MySqlCommand GetCommand(string clientID)
            {
                string sql = "SELECT * FROM ClientConfigs WHERE ClientID = @clientID";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "@clientID", clientID);

                return cmd;
            }
        }

        static void AddParameter(MySqlCommand cmd, string key, object value)
        {
            cmd.Parameters.Add(new MySqlParameter(key, value));
        }
    }
}
