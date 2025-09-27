namespace CSVTransferApp.Services.Tests.Mocks;

public class MockSftpClient : IDisposable
{
    public bool IsConnected { get; private set; }
    public List<string> UploadedFiles { get; } = new();
    public Dictionary<string, byte[]> FileContents { get; } = new();
    public bool ShouldThrowOnConnect { get; set; }
    public bool ShouldThrowOnUpload { get; set; }

    public void Connect()
    {
        if (ShouldThrowOnConnect)
            throw new InvalidOperationException("Mock connection error");
            
        IsConnected = true;
    }

    public void Disconnect()
    {
        IsConnected = false;
    }

    public void UploadFile(Stream input, string path)
    {
        if (!IsConnected)
            throw new InvalidOperationException("Not connected");
            
        if (ShouldThrowOnUpload)
            throw new InvalidOperationException("Mock upload error");

        using var memoryStream = new MemoryStream();
        input.CopyTo(memoryStream);
        
        var fileName = Path.GetFileName(path);
        UploadedFiles.Add(fileName);
        FileContents[fileName] = memoryStream.ToArray();
    }

    public bool Exists(string path)
    {
        var fileName = Path.GetFileName(path);
        return UploadedFiles.Contains(fileName);
    }

    public void DeleteFile(string path)
    {
        var fileName = Path.GetFileName(path);
        UploadedFiles.Remove(fileName);
        FileContents.Remove(fileName);
    }

    public void Dispose()
    {
        IsConnected = false;
        UploadedFiles.Clear();
        FileContents.Clear();
        GC.SuppressFinalize(this);
    }
}
