using System.Collections.Concurrent;
using System.Text.Json;

namespace JsonServer
{
    public class Database
    {
        public ConcurrentDictionary<string, List<Dictionary<string, object>>> Tables { get; set; }

        private readonly string FilePath;

        private readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

        public Database(string filePath)
        {
            FilePath = filePath;
            Tables = [];

            Seed();
        }

        public IEnumerable<string> TableNames => Tables.Keys;

        public Dictionary<string, object>? GetById(string id, string tableName)
        {
            if (!Tables.TryGetValue(tableName, out var rows))
            {
                return null;
            }

            var row = rows.FirstOrDefault(x => x["id"].ToString() == id);

            if (row == null)
            {
                return null;
            }

            return row;
        }

        public Dictionary<string, object>? Insert(JsonElement element, string tableName)
        {
            if (!Tables.TryGetValue(tableName, out var rows))
            {
                return null;
            }

            var row = JsonSerializer.Deserialize<Dictionary<string, object>>(element.GetRawText());

            if (row != null)
            {
                rows.Add(row);
            }

            return row;
        }

        public Dictionary<string, object>? Update(string id, JsonElement element, string tableName)
        {
            if (!Delete(id, tableName))
            {
                return null;
            }

            return Insert(element, tableName);
        }

        public bool Delete(string id, string tableName)
        {
            if (!Tables.TryGetValue(tableName, out var rows))
            {
                return false;
            }

            var row = rows.FirstOrDefault(x => x["id"].ToString() == id);

            if (row == null)
            {
                return false;
            }

            return rows.Remove(row);
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(Tables, SerializerOptions);

                await File.WriteAllTextAsync(FilePath, jsonString);
            }
            catch (Exception)
            {
                // 
            }
        }

        private void Seed()
        {
            using var stream = File.OpenRead(FilePath);
            using var doc = JsonDocument.Parse(stream);

            foreach (var table in doc.RootElement.EnumerateObject())
            {
                var rows = new List<Dictionary<string, object>>();

                if (table.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var element in table.Value.EnumerateArray())
                    {
                        var row = JsonSerializer.Deserialize<Dictionary<string, object>>(element.GetRawText());

                        if (row != null)
                        {
                            rows.Add(row);
                        }
                    }

                    Tables.TryAdd(table.Name, rows);
                }
            }
        }
    }
}
