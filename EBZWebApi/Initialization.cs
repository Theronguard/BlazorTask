using EBZWebApi.DataAccess;
using System.Text;
using EBZShared.Models;
using System.Text.Json;

namespace EBZWebApi
{
    /// <summary>
    /// Class used for the application initialization
    /// </summary>
    public class Initialization
    {
        private const string? _dataSeedFileName = "data_seed.json";
        private const int _bufferSize = 1024;

        #region Methods

        /// <summary>
        /// Initializes SQLite  
        /// </summary>
        public static void Initialize()
        {
            if (!Directory.Exists(Settings.DBPath))
                Directory.CreateDirectory(Settings.DBPath);

            SQLitePCL.Batteries.Init();

            using UserDA userDA = new();
            InitializeUserDatabase(userDA);
        }

        /// <summary>
        /// Reads the default user from the .json file
        /// </summary>
        /// <returns></returns>
        private static User? ReadUserFromSeed()
        {
            string dataSeedFilePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, _dataSeedFileName);
            if (!File.Exists(dataSeedFilePath)) return null;

            using FileStream fs = new(dataSeedFilePath, FileMode.Open);

            StringBuilder readString = new();

            byte[] buffer = new byte[_bufferSize];
            int readBytes = 1;
            while ((readBytes = fs.Read(buffer, 0, _bufferSize)) > 0)
                readString.Append(Encoding.UTF8.GetString(buffer,0, readBytes));

            JsonSerializerOptions options = new();
            options.PropertyNameCaseInsensitive = true;

            return JsonSerializer.Deserialize<User>(readString.ToString(), options);
        }

        /// <summary>
        /// Ensures a DB is created, and an initial record is created
        /// </summary>
        /// <param name="userDA"></param>
        private static void InitializeUserDatabase(UserDA userDA)
        {
            userDA.Database.EnsureCreated();

            if (userDA.Users.Count() > 0) return;

            User? user = ReadUserFromSeed();

            if (user is null) return;

            userDA.Add(user);
            userDA.SaveChanges();
        }

        #endregion
    }
}
