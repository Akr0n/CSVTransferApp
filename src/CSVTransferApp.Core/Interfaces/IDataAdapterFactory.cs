using System.Data;

namespace CSVTransferApp.Core.Interfaces;

/// <summary>
/// Factory per la creazione di data adapter specifici per provider database
/// </summary>
public interface IDataAdapterFactory
{
    /// <summary>
    /// Crea un data adapter per il provider specificato
    /// </summary>
    /// <param name="providerName">Nome del provider database (es. "Oracle", "SqlServer", "PostgreSQL")</param>
    /// <param name="command">Comando da associare al data adapter</param>
    /// <returns>Data adapter specifico per il provider</returns>
    /// <exception cref="ArgumentException">Se il provider non è supportato</exception>
    IDbDataAdapter CreateDataAdapter(string providerName, IDbCommand command);

    /// <summary>
    /// Verifica se un provider è supportato
    /// </summary>
    /// <param name="providerName">Nome del provider da verificare</param>
    /// <returns>true se il provider è supportato, false altrimenti</returns>
    bool IsProviderSupported(string providerName);
}