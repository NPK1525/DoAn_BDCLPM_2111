using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestProject1.Utilities
{
    public static class ConfigReader
    {
        private static readonly Lazy<UsersConfig> _users = new(LoadUsers);

        private static UsersConfig LoadUsers()
        {
            // Tìm file users.json: ưu tiên thư mục TestData cạnh executable
            string[] searchPaths =
            [
                Path.Combine(AppContext.BaseDirectory, "TestData", "users.json"),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "TestData", "users.json")
            ];

            foreach (var path in searchPaths)
            {
                string fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    string json = File.ReadAllText(fullPath);
                    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
                    return JsonSerializer.Deserialize<UsersConfig>(json, options)
                           ?? throw new InvalidOperationException("Cannot deserialize users.json");
                }
            }

            throw new FileNotFoundException($"users.json not found. Searched: {string.Join(", ", searchPaths)}");
        }

        /// <summary>
        /// Lấy thông tin tài khoản theo key: "admin", "user", "empty_user"
        /// </summary>
        public static UserAccount GetUserData(string key)
        {
            return key.ToLowerInvariant() switch
            {
                "admin"      => _users.Value.Admin,
                "user"       => _users.Value.User,
                "empty_user" => _users.Value.EmptyUser,
                _            => throw new ArgumentException($"Unknown user key: '{key}'. Valid keys: admin, user, empty_user")
            };
        }
    }

    // Ánh xạ cấu trúc JSON: { "admin": {...}, "user": {...}, "empty_user": {...} }
    internal class UsersConfig
    {
        [JsonPropertyName("admin")]      public UserAccount Admin     { get; set; } = new();
        [JsonPropertyName("user")]       public UserAccount User      { get; set; } = new();
        [JsonPropertyName("empty_user")] public UserAccount EmptyUser { get; set; } = new();
    }
}
