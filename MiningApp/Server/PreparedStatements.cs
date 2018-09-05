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
        static MySqlConnection _connection => ServerHelper.Instance.GetDatabaseConnection;

        public class UpdateClient
        {
            public static MySqlCommand GetCommand(string id, string userID, DateTime timestamp)
            {
                string sql = "INSERT INTO Clients (@id, @userID, @timestamp) ON DEUPLICATE KEY UPDATE ClientID=@id, timestamp=@timestamp";

                var cmd = new MySqlCommand(sql, _connection);
                AddParameter(cmd, "@id", id);
                AddParameter(cmd, "@userID", userID);
                AddParameter(cmd, "@timestamp", timestamp);

                return cmd;
            }
        }

        static void AddParameter(MySqlCommand cmd, string key, object value)
        {
            cmd.Parameters.Add(new MySqlParameter(key, value));
        }
    }
}
