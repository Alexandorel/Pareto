using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ParetoApp
{
    public class BoardStorage
    {
        private readonly string _folderPath;
        private readonly JsonSerializerOptions _jsonOptions;

        public BoardStorage()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _folderPath = Path.Combine(appData, "ParetoApp", "Boards");
            Directory.CreateDirectory(_folderPath);

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        public void Save(BoardData board)
        {
            try
            {
                string filePath = Path.Combine(_folderPath, $"{board.Name}.json");
                string json = JsonSerializer.Serialize(board, _jsonOptions);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not save board '{board.Name}': {ex.Message}", ex);
            }
        }

        public BoardData? Load(string name)
        {
            try
            {
                string filePath = Path.Combine(_folderPath, $"{name}.json");
                if (!File.Exists(filePath)) return null;

                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<BoardData>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not load board '{name}': {ex.Message}", ex);
            }
        }

        public List<string> ListBoards()
        {
            try
            {
                return Directory.GetFiles(_folderPath, "*.json")
                    .Select(Path.GetFileNameWithoutExtension)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Select(n => n!)
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        public void Delete(string name)
        {
            try
            {
                string filePath = Path.Combine(_folderPath, $"{name}.json");
                if (File.Exists(filePath)) File.Delete(filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not delete board '{name}': {ex.Message}", ex);
            }
        }

        public string FolderPath => _folderPath;
    }
}
