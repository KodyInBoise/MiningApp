using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class DatabaseHelper
    {
        MySqlConnection _connection { get; set; }

        string _user = "mining-client";
        string _password = "db)T8WaDAJkWz7qtRodc@jbd";

        public DatabaseHelper()
        {
            _connection = new MySqlConnection(GetConnectionString());
        }

        public MySqlConnection GetConnection()
        {
            return _connection;
        }

        private string GetConnectionString()
        {
            string s = $"Server=23.229.226.104; Database=mining-app; Uid={_user}; Pwd={_password}; SslMode=none";
            return s;
        }

        public string GetNewClientID()
        {
            return Guid.NewGuid().ToString();
        }

        public bool VerifyNewID(string id)
        {
            return true;
        }

        public async Task UpdateClient()
        {
            var cmd = PreparedStatements.UpdateClient.GetCommand(Bootstrapper.Settings.Server.LocalClientID, Bootstrapper.Settings.User.UserID, DateTime.Now);

            await _connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        public async Task<LocalClientModel> GetClientInfo(string clientID)
        {
            var client = new LocalClientModel();
            var cmd = PreparedStatements.GetClient.GetCommand(clientID);

            using (_connection)
            {
                await _connection.OpenAsync();

                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync())
                    {
                        client.ID = rdr.GetString(Indexes.Clients.ClientID);
                        client.LastCheckin = rdr.GetDateTime(Indexes.Clients.LastCheckin);
                    }
                }
            }

            return client;
        }

        public async Task UpdateUser(UserModel user)
        {
            var cmd = PreparedStatements.UpdateUser.GetCommand(user);

            using (_connection) 
            {
                await _connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await _connection.CloseAsync();
            }
        }
    }
}
