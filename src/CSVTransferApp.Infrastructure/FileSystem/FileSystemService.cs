using CSVTransferApp.Core.Extensions;

namespace CSVTransferApp.Infrastructure.FileSystem;

public class FileSystemService
{
    public async Task<string> ReadAllTextAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        return await File.ReadAllTextAsync(filePath);
    }

    public async Task WriteAllTextAsync(string filePath, string content)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(filePath, content);
    }

    public async Task AppendAllTextAsync(string filePath, string content)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.AppendAllTextAsync(filePath, content);
    }

    public bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    public bool DirectoryExists(string directoryPath)
    {
        return Directory.Exists(directoryPath);
    }

    public void CreateDirectory(string directoryPath)
    {
        Directory.CreateDirectory(directoryPath);
    }

    public void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public string GetSafeFileName(string fileName)
    {
        return fileName.ToSafeFileName();
    }

    public long GetFileSize(string filePath)
    {
        if (!File.Exists(filePath))
            return 0;

        return new FileInfo(filePath).Length;
    }

    public DateTime GetLastWriteTime(string filePath)
    {
        if (!File.Exists(filePath))
            return DateTime.MinValue;

        return File.GetLastWriteTime(filePath);
    }

    public IEnumerable<string> GetFiles(string directoryPath, string searchPattern = "*.*")
    {
        if (!Directory.Exists(directoryPath))
            return Enumerable.Empty<string>();

        return Directory.GetFiles(directoryPath, searchPattern);
    }
}
