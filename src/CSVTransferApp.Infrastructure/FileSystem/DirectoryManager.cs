namespace CSVTransferApp.Infrastructure.FileSystem;

public class DirectoryManager
{
    private readonly Dictionary<string, string> _managedDirectories;

    public DirectoryManager()
    {
        _managedDirectories = new Dictionary<string, string>();
    }

    public void RegisterDirectory(string name, string path)
    {
        _managedDirectories[name] = path;
        EnsureDirectoryExists(path);
    }

    public string GetDirectory(string name)
    {
        if (!_managedDirectories.TryGetValue(name, out var path))
        {
            throw new ArgumentException($"Directory '{name}' is not registered");
        }

        return path;
    }

    public void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public void CleanDirectory(string name, TimeSpan olderThan)
    {
        var path = GetDirectory(name);
        var cutoffTime = DateTime.UtcNow - olderThan;

        var filesToDelete = Directory.GetFiles(path)
            .Where(file => File.GetLastWriteTimeUtc(file) < cutoffTime)
            .ToList();

        foreach (var file in filesToDelete)
        {
            try
            {
                File.Delete(file);
            }
            catch
            {
                // Ignore deletion errors
            }
        }
    }

    public long GetDirectorySize(string name)
    {
        var path = GetDirectory(name);
        if (!Directory.Exists(path))
            return 0;

        return Directory.GetFiles(path, "*", SearchOption.AllDirectories)
            .Sum(file => new FileInfo(file).Length);
    }

    public void CreateBackup(string name, string backupSuffix = null!)
    {
        var sourcePath = GetDirectory(name);
        var backupPath = sourcePath + (backupSuffix ?? $"_backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}");

        if (Directory.Exists(sourcePath))
        {
            CopyDirectory(sourcePath, backupPath);
        }
    }

    private static void CopyDirectory(string sourcePath, string targetPath)
    {
        Directory.CreateDirectory(targetPath);

        foreach (var file in Directory.GetFiles(sourcePath))
        {
            var fileName = Path.GetFileName(file);
            var targetFile = Path.Combine(targetPath, fileName);
            File.Copy(file, targetFile, true);
        }

        foreach (var directory in Directory.GetDirectories(sourcePath))
        {
            var directoryName = Path.GetFileName(directory);
            var targetDirectory = Path.Combine(targetPath, directoryName);
            CopyDirectory(directory, targetDirectory);
        }
    }
}
