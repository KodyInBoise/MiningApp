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
            }

            public static string GetColumnName(Columns column)
            {
                switch (column)
                {
                    case Columns.ClientID:
                        return ColumnnNames.Clients.ClientID;
                    case Columns.UserID:
                        return ColumnnNames.Clients.UserID;
                    case Columns.LastCheckin:
                        return ColumnnNames.Clients.LastCheckin;
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
            }

            public static string GetColumnName(Columns column)
            {
                switch (column)
                {
                    case Columns.UserID:
                        return ColumnnNames.Users.UserID;
                    case Columns.Email:
                        return ColumnnNames.Users.Email;
                    case Columns.Password:
                        return ColumnnNames.Users.Password;
                    case Columns.Created:
                        return ColumnnNames.Users.Created;
                    case Columns.LastLogin:
                        return ColumnnNames.Users.LastLogin;
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

    public static class ColumnnNames
    {
        public static class Clients
        {
            public static string ClientID = "ClientID";
            public static string UserID = "UserID";
            public static string LastCheckin = "LastCheckin";
        }

        public static class Users
        {
            public static string UserID = "UserID";
            public static string Email = "Email";
            public static string Password = "Password";
            public static string Created = "Created";
            public static string LastLogin = "LastLogin";
        }
    }
}
