using System.Security.Cryptography;
using System.Text;

namespace CSVTransferApp.Infrastructure.Security;

public class CredentialManager
{
    private readonly EncryptionService _encryptionService;

    public CredentialManager(EncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public void StoreCredential(string key, string value)
    {
        var encryptedValue = _encryptionService.Encrypt(value);
        
        // In production, store in secure credential store (Windows Credential Manager, etc.)
        Environment.SetEnvironmentVariable($"ENCRYPTED_{key}", encryptedValue, EnvironmentVariableTarget.User);
    }

    public string? GetCredential(string key)
    {
        var encryptedValue = Environment.GetEnvironmentVariable($"ENCRYPTED_{key}", EnvironmentVariableTarget.User);
        
        if (string.IsNullOrEmpty(encryptedValue))
            return null;

        return _encryptionService.Decrypt(encryptedValue);
    }

    public void RemoveCredential(string key)
    {
        Environment.SetEnvironmentVariable($"ENCRYPTED_{key}", null, EnvironmentVariableTarget.User);
    }

    public bool HasCredential(string key)
    {
        return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable($"ENCRYPTED_{key}", EnvironmentVariableTarget.User));
    }

    public Dictionary<string, string> GetAllCredentials(string prefix = "")
    {
        var credentials = new Dictionary<string, string>();
        var envVars = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User);

        foreach (System.Collections.DictionaryEntry entry in envVars)
        {
            var key = entry.Key.ToString()!;
            if (key.StartsWith("ENCRYPTED_" + prefix))
            {
                var credentialKey = key.Substring(10); // Remove "ENCRYPTED_" prefix
                var decryptedValue = _encryptionService.Decrypt(entry.Value?.ToString() ?? "");
                credentials[credentialKey] = decryptedValue;
            }
        }

        return credentials;
    }
}
