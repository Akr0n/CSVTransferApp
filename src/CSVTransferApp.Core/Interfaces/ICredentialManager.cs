namespace CSVTransferApp.Core.Interfaces;

public interface ICredentialManager
{
    string GetCredential(string key);
    void SaveCredential(string key, string value);
}