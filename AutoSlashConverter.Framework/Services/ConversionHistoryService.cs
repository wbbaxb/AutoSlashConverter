using AutoSlashConverter.Framework.Models;
using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace AutoSlashConverter.Framework.Services
{
    public class ConversionHistoryService
    {
        private readonly ObservableCollection<ConversionHistory> _histories;
        private readonly string _historyFilePath;
        private const int MaxHistoryCount = 200;

        public ConversionHistoryService()
        {
            _histories = new ObservableCollection<ConversionHistory>();

            // 历史记录文件保存在用户文档目录
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var appDataPath = Path.Combine(documentsPath, "AutoSlashConverter");
            Directory.CreateDirectory(appDataPath);
            _historyFilePath = Path.Combine(appDataPath, "conversion_history.json");

            LoadHistories();
        }

        public ObservableCollection<ConversionHistory> Histories => _histories;

        public void AddHistory(string originalPath, string convertedPath)
        {
            if (string.IsNullOrWhiteSpace(originalPath) || string.IsNullOrWhiteSpace(convertedPath))
                return;

            // 避免重复记录相同的转换
            var existing = _histories.Any(h =>
                h.OriginalPath.Equals(originalPath, StringComparison.OrdinalIgnoreCase) &&
                h.ConvertedPath.Equals(convertedPath, StringComparison.OrdinalIgnoreCase));

            if (!existing)
            {
                // 添加新记录到最前面
                var newHistory = new ConversionHistory(originalPath, convertedPath);
                _histories.Insert(0, newHistory);

                // 限制历史记录数量
                while (_histories.Count > MaxHistoryCount)
                {
                    _histories.RemoveAt(_histories.Count - 1);
                }

                SaveHistories();
            }
        }

        public void ClearHistories()
        {
            _histories.Clear();
            SaveHistories();
        }

        private void LoadHistories()
        {
            try
            {
                if (File.Exists(_historyFilePath))
                {
                    var json = File.ReadAllText(_historyFilePath);
                    var histories = JsonSerializer.Deserialize<List<ConversionHistory>>(json);

                    if (histories != null)
                    {
                        _histories.Clear();
                        foreach (var history in histories.OrderByDescending(h => h.ConvertedTime))
                        {
                            _histories.Add(history);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载历史记录失败: {ex.Message}");
            }
        }

        private void SaveHistories()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                var json = JsonSerializer.Serialize(_histories.ToList(), options);
                File.WriteAllText(_historyFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存历史记录失败: {ex.Message}");
            }
        }
    }
}