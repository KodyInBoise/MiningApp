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

        public async Task UpdateClient(LocalClientModel client, string userID)
        {
            try
            {
                var cmd = PreparedStatements.UpdateClient.GetCommand(client, userID);

                await _connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await _connection.CloseAsync();
            }
            catch (Exception ex)
            {
                ExceptionUtil.Handle(ex);
            }
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
                        client.ID = rdr.GetString(DBInfo.Clients.GetColumnIndex(DBInfo.Clients.Columns.ClientID));
                        client.LastCheckin = rdr.GetDateTime(DBInfo.Clients.GetColumnIndex(DBInfo.Clients.Columns.LastCheckin));
                        client.PublicIP = rdr.GetString(DBInfo.Clients.GetColumnIndex(DBInfo.Clients.Columns.PublicIP));
                        client.PrivateIP = rdr.GetString(DBInfo.Clients.GetColumnIndex(DBInfo.Clients.Columns.PrivateIP));
                    }
                }
            }

            return client;
        }

        UserModel _cachedLocalUser { get; set; }
        public async Task<UserModel> GetUser(string email, bool refresh = false)
        {
            try
            {
                if (!refresh)
                {
                    if (_cachedLocalUser != null && _cachedLocalUser.Email == email)
                    {
                        return _cachedLocalUser;
                    }
                }

                var cmd = PreparedStatements.GetUser.GetCommand(email);
                var user = new UserModel();

                using (_connection)
                {
                    await _connection.OpenAsync();

                    using (var rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            user.ID = rdr.GetString(DBInfo.Users.GetColumnIndex(DBInfo.Users.Columns.UserID));
                            user.Email = rdr.GetString(DBInfo.Users.GetColumnIndex(DBInfo.Users.Columns.Email));
                            user.Password = rdr.GetString(DBInfo.Users.GetColumnIndex(DBInfo.Users.Columns.Password));
                            user.LastServerLogin = rdr.GetDateTime(DBInfo.Users.GetColumnIndex(DBInfo.Users.Columns.LastLogin));
                            user.RequiresLogin = rdr.GetBoolean(DBInfo.Users.GetColumnIndex(DBInfo.Users.Columns.RequiresLogin));
                        }
                    }
                }

                _cachedLocalUser = user;

                return user;
            }
            catch (Exception ex)
            {
                HandleServerException(ex);

                return null;
            }
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

        public async Task<bool> AuthenticateUser(string email, string password)
        {
            var user = await GetUser(email);

            if (user == null || user.Email != email)
            {
                Bootstrapper.UserAuthenticationDelegate?.Invoke(new UserAuthenticationChangedArgs(UserAuthenticationStatus.Disconnected));

                return false;
            }

            if (user.Password == password)
            {
                Bootstrapper.Instance.SetUser(user, true);
                Bootstrapper.UserAuthenticationDelegate?.Invoke(new UserAuthenticationChangedArgs(UserAuthenticationStatus.Connected));

                return true;
            }
            else
            {
                Bootstrapper.UserAuthenticationDelegate?.Invoke(new UserAuthenticationChangedArgs(UserAuthenticationStatus.Disconnected));

                return false;
            }
        }

        public async Task<List<LocalClientModel>> GetUserClients(string userID)
        {
            var clients = new List<LocalClientModel>();

            var cmd = PreparedStatements.GetUserClients.GetCommand(userID);

            using (_connection)
            {
                await _connection.OpenAsync();

                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync())
                    {
                        var client = new LocalClientModel()
                        {
                            ID = rdr.GetString(DBInfo.Clients.GetColumnIndex(DBInfo.Clients.Columns.ClientID)),
                            LastCheckin = rdr.GetDateTime(DBInfo.Clients.GetColumnIndex(DBInfo.Clients.Columns.LastCheckin))
                        };

                        clients.Add(client);
                    }
                }

                await _connection.CloseAsync();
            }

            return clients;
        }

        public async Task InsertClientMessage(ClientMessageModel message)
        {
            var cmd = PreparedStatements.InsertClientMessage.GetCommand(message);

            using (_connection)
            {
                await _connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await _connection.CloseAsync();
            }
        }

        public async Task<List<ClientMessageModel>> GetClientMessages(string clientID)
        {
            var messages = new List<ClientMessageModel>();

            var cmd = PreparedStatements.GetClientMessages.GetCommand(clientID);

            using (_connection)
            {
                await _connection.OpenAsync();

                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync())
                    {
                        var msg = new ClientMessageModel()
                        {
                            ClientID = rdr.GetString(DBInfo.ClientMessages.GetColumnIndex(DBInfo.ClientMessages.Columns.ClientID)),
                            Timestamp = rdr.GetDateTime(DBInfo.ClientMessages.GetColumnIndex(DBInfo.ClientMessages.Columns.Timestamp)),
                            Message = rdr.GetString(DBInfo.ClientMessages.GetColumnIndex(DBInfo.ClientMessages.Columns.Message)),
                            Action = ClientAction.GetAction(rdr.GetString(DBInfo.ClientMessages.GetColumnIndex(DBInfo.ClientMessages.Columns.Action))),
                        };

                        messages.Add(msg);
                    }
                }

                await _connection.CloseAsync();
            }

            return messages;
        }

        void HandleServerException(Exception ex)
        {

        }
    }
}
