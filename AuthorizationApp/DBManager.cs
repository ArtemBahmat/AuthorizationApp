using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace AuthorizationApp
{
    static class DBManager
    {
        static DBManager()
        {
            string path = GetDBPath();
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            string fullDBPath = path + "\\" + GetDBName();

            if (!File.Exists(fullDBPath))
            {
                InitializeDB();
            }
        }

        private static void InitializeDB()
        {
            List<User> users = GenerateUsers();
            SaveUsers(users);
        }

        public static bool IsUserExists(string login)
        {
            return GetAllUsers().Where(user => user.Login == login).Any();
        }
        public static bool AreKeyValueExistInDB(string expectedKey, string expectedValue)
        {
            return DBManager.GetAllUsers().Where(user => user.Login == expectedKey && CryptoHelper.Decrypt(user.Password) == expectedValue).Any();
        }

        public static void SaveUsers(List<User> users)
        {
            if (users != null)
            {
                try
                {
                    using (UserContext db = new UserContext())
                    {
                        db.Users.AddRange(users);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public static bool SaveUser(User user)
        {
            bool success = false;
            try
            {
                user.Password = CryptoHelper.Encrypt(user.Password);

                using (UserContext db = new UserContext())
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    success = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return success;
        }

        public static List<User> GetAllUsers()
        {
            List<User> result = new List<User>();

            try
            {
                using (var db = new UserContext())
                {
                    result = db.Users.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;
        }

        private static List<User> GenerateUsers()
        {
            return new List<User>
                {
                      new User("login1", CryptoHelper.Encrypt("password1"), "Ivanov"),
                      new User("login2", CryptoHelper.Encrypt("password2"), "Petrov"),
                      new User("login3", CryptoHelper.Encrypt("password3"), "Sidorov"),
                      new User("login4", CryptoHelper.Encrypt("password4"), "Konstantinov"),
                      new User("login5", CryptoHelper.Encrypt("password5"), "Nikolaev"),
                      new User("login6", CryptoHelper.Encrypt("password6"), "Smirnov"),
                };
        }

        private static string GetDBPath()
        {
            string relative = @"..\..\App_Data";
            string absolute = Path.GetFullPath(relative);
            return absolute;
        }

        private static string GetDBName()
        {
            string connectionString = GetConnectionString();
            int startIndex = connectionString.LastIndexOf("\\");
            int endIndex = connectionString.LastIndexOf("'") - 1;
            string fileName = connectionString.Substring(startIndex + 1, endIndex - startIndex);
            return fileName;
        }

        private static string GetConnectionString()
        {
            string connectionString = string.Empty;

            try
            {
                using (var db = new UserContext())
                {
                    connectionString = db.Database.Connection.ConnectionString;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return connectionString;
        }
    }
}
