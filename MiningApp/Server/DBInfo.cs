using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class DBInfo
    {
        public class Clients
        {
            public enum Columns
            {
                ClientID = 0,
                UserID = 1,
                LastCheckin = 2,
                PublicIP = 3,
                PrivateIP = 4,
            }

            public static string GetColumnName(Columns column)
            {
                switch (column)
                {
                    case Columns.ClientID:
                        return ColumnNames.Clients.ClientID;
                    case Columns.UserID:
                        return ColumnNames.Clients.UserID;
                    case Columns.LastCheckin:
                        return ColumnNames.Clients.LastCheckin;
                    case Columns.PublicIP:
                        return ColumnNames.Clients.PublicIP;
                    case Columns.PrivateIP:
                        return ColumnNames.Clients.PrivateIP;
                    default:
                        return string.Empty;
                }
            }

            public static string GetColumnName(int index)
            {
                var columnEnum = (Columns)index;

                return GetColumnName(columnEnum);
            }

            public static int GetColumnIndex(Columns column)
            {
                return (int)column;
            }
        }

        public class Users
        {
            public enum Columns
            {
                UserID = 0,
                Email = 1,
                Password = 2,
                Created = 3,
                LastLogin = 4,
                RequiresLogin = 5,
            }

            public static string GetColumnName(Columns column)
            {
                switch (column)
                {
                    case Columns.UserID:
                        return ColumnNames.Users.UserID;
                    case Columns.Email:
                        return ColumnNames.Users.Email;
                    case Columns.Password:
                        return ColumnNames.Users.Password;
                    case Columns.Created:
                        return ColumnNames.Users.Created;
                    case Columns.LastLogin:
                        return ColumnNames.Users.LastLogin;
                    case Columns.RequiresLogin:
                        return ColumnNames.Users.RequiresLogin;
                    default:
                        return string.Empty;
                }
            }

            public static string GetColumnName(int index)
            {
                var columnEnum = (Columns)index;

                return GetColumnName(columnEnum);
            }

            public static int GetColumnIndex(Columns column)
            {
                return (int)column;
            }
        }

        public class ClientMessages
        {
            public enum Columns
            {
                ClientID = 0,
                Timestamp = 1,
                Message = 2,
                Action = 3,
            }

            public static string GetColumnName(Columns column)
            {
                switch (column)
                {
                    case Columns.ClientID:
                        return ColumnNames.ClientMessages.ClientID;
                    case Columns.Timestamp:
                        return ColumnNames.ClientMessages.Timestamp;
                    case Columns.Message:
                        return ColumnNames.ClientMessages.Message;
                    case Columns.Action:
                        return ColumnNames.ClientMessages.Action;
                    default:
                        return string.Empty;
                }
            }

            public static string GetColumnName(int index)
            {
                var columnEnum = (Columns)index;

                return GetColumnName(columnEnum);
            }

            public static int GetColumnIndex(Columns column)
            {
                return (int)column;
            }
        }
    }

    public static class ColumnNames
    {
        public static class Clients
        {
            public static string ClientID = "ClientID";
            public static string UserID = "UserID";
            public static string LastCheckin = "LastCheckin";
            public static string PublicIP = "PublicIP";
            public static string PrivateIP = "PrivateIP";
        }

        public static class Users
        {
            public static string UserID = "UserID";
            public static string Email = "Email";
            public static string Password = "Password";
            public static string Created = "Created";
            public static string LastLogin = "LastLogin";
            public static string RequiresLogin = "RequiresLogin";
        }

        public static class ClientMessages
        {
            public static string ClientID = "ClientID";
            public static string Timestamp = "Timestamp";
            public static string Message = "Message";
            public static string Action = "Action";
        }
    }
}
