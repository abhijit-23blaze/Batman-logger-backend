using hello_world_api.Models;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace hello_world_api.Services
{
    public class JournalService
    {
        private List<JournalEntry> _entries = new List<JournalEntry>();
        private static int _nextId = 1;
        private readonly string _filePath;
        private readonly ILogger<JournalService> _logger;
        private readonly byte[] _encryptionKey;
        private readonly byte[] _encryptionIv;

        public JournalService(IConfiguration configuration, ILogger<JournalService> logger)
        {
            // Get the data directory from configuration or use a default
            var dataDir = configuration["JournalSettings:DataDirectory"] ?? "App_Data";
            
            // Ensure directory exists
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
            
            _filePath = Path.Combine(dataDir, "journal-entries.json");
            _logger = logger;
            
            // Get or generate encryption key and IV
            string keyString = configuration["JournalSettings:EncryptionKey"] ?? "YourSuperSecretKey123456789012345678";
            string ivString = configuration["JournalSettings:EncryptionIV"] ?? "1234567890123456";
            
            // Ensure the key and IV are of appropriate length for AES
            _encryptionKey = Encoding.UTF8.GetBytes(keyString).Take(32).ToArray(); // 256 bits = 32 bytes
            _encryptionIv = Encoding.UTF8.GetBytes(ivString).Take(16).ToArray();  // 128 bits = 16 bytes
            
            // Load existing entries when service starts
            LoadEntries();
        }

        private void LoadEntries()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    // Read encrypted data
                    byte[] encryptedData = File.ReadAllBytes(_filePath);
                    
                    // Decrypt the data
                    string json = DecryptData(encryptedData);
                    
                    // Deserialize JSON to entries
                    var entries = JsonSerializer.Deserialize<List<JournalEntry>>(json);
                    
                    if (entries != null && entries.Any())
                    {
                        _entries = entries;
                        _nextId = _entries.Max(e => e.Id) + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading journal entries from file");
            }
        }

        private void SaveEntries()
        {
            try
            {
                // Serialize to JSON
                var json = JsonSerializer.Serialize(_entries, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                
                // Encrypt the JSON string
                byte[] encryptedData = EncryptData(json);
                
                // Write encrypted data to file
                File.WriteAllBytes(_filePath, encryptedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving journal entries to file");
            }
        }
        
        private byte[] EncryptData(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _encryptionKey;
                aes.IV = _encryptionIv;
                
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptoStream))
                        {
                            writer.Write(plainText);
                        }
                        
                        return memoryStream.ToArray();
                    }
                }
            }
        }
        
        private string DecryptData(byte[] cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _encryptionKey;
                aes.IV = _encryptionIv;
                
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                
                using (MemoryStream memoryStream = new MemoryStream(cipherText))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public List<JournalEntry> GetEntries(int limit = 0)
        {
            var entries = _entries.OrderByDescending(e => e.Timestamp).ToList();
            
            if (limit > 0)
            {
                entries = entries.Take(limit).ToList();
            }
            
            return entries;
        }

        public List<JournalEntry> GetEntriesByDate(DateTime date)
        {
            return _entries
                .Where(e => e.Timestamp.Date == date.Date)
                .OrderByDescending(e => e.Timestamp)
                .ToList();
        }

        public JournalEntry AddEntry(string content)
        {
            var entry = new JournalEntry
            {
                Id = _nextId++,
                Content = content,
                Timestamp = DateTime.UtcNow
            };
            
            _entries.Add(entry);
            
            // Save entries to file after adding a new one
            SaveEntries();
            
            return entry;
        }
    }
} 