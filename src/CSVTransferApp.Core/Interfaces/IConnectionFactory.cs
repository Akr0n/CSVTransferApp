using System.Data;

namespace CSVTransferApp.Core.Interfaces;

/// <summary>
/// Factory per la creazione di connessioni database specifiche per provider
/// </summary>
public interface IConnectionFactory
{
    /// <summary>
    /// Crea una connessione database per il provider specificato
    /// </summary>
    /// <param name="connectionString">Stringa di connessione</param>
    /// <param name="providerName">Nome del provider (es. "Oracle", "SqlServer", "PostgreSQL")</param>
    /// <returns>Connessione database specifica per il provider</returns>
    /// <exception cref="ArgumentException">Se il provider non è supportato</exception>
    IDbConnection CreateConnection(string connectionString, string providerName);

    /// <summary>
    /// Verifica se un provider è supportato
    /// </summary>
    /// <param name="providerName">Nome del provider da verificare</param>
    /// <returns>true se il provider è supportato, false altrimenti</returns>
    bool IsProviderSupported(string providerName);
}