using System.IO;
using System.Text.Json;

namespace SanGuoCharacterEditor.Utils
{
    public sealed class SettingsManager<T> where T : class
    {
        private readonly string _path;

        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true
        };

        private readonly object _lock = new();

        private CancellationTokenSource? _debounceCts;

        private readonly int _debounceDelay = 300;

        public T Value { get; private set; }

        public SettingsManager(T value, string path)
        {
            Value = value;
            _path = path;
            Load();
        }

        public void Update(Func<T, T> updater)
        {
            lock (_lock)
            {
                Value = updater(Value);
                ScheduleSave();
            }
        }

        private void Load()
        {
            if (!File.Exists(_path))
                return;
            try
            {
                var json = File.ReadAllText(_path);
                var obj = JsonSerializer.Deserialize<T>(json, _options);
                if (obj is not null)
                    Value = obj;
            }
            catch { }
        }

        private void ScheduleSave()
        {
            _debounceCts?.Cancel();
            _debounceCts = new CancellationTokenSource();

            var token = _debounceCts.Token;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(_debounceDelay, token);
                    Save();
                }
                catch (TaskCanceledException) { }
            });
        }

        public void Save()
        {
            lock (_lock)
            {
                SafeWriteJson(_path, Value, _options);
            }
        }

        public static void SafeWriteJson(string path, T value, JsonSerializerOptions options)
        {
            var dir = Path.GetDirectoryName(Path.GetFullPath(path));

            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                return;

            var tempPath = path + ".tmp";

            using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                JsonSerializer.Serialize(fs, value, options);
                fs.Flush(true);
            }

            if (File.Exists(path))
                File.Replace(tempPath, path, null);
            else
                File.Move(tempPath, path);
        }
    }
}