namespace CSVTransferApp.Core.Models;

/// <summary>
/// Configurazione per personalizzare le intestazioni CSV
/// </summary>
public class HeaderOverride
{
    /// <summary>
    /// Nome della tabella a cui si applica questo override
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Descrizione dell'override per documentazione
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Mappatura tra nome colonna database e nome colonna CSV
    /// Key = nome colonna database, Value = nome colonna CSV
    /// </summary>
    public Dictionary<string, string> ColumnMappings { get; set; } = new();

    /// <summary>
    /// Tipi di dato per le colonne (per validazione e formattazione)
    /// Key = nome colonna CSV, Value = tipo di dato (string)
    /// Nota: usiamo string per compatibilità con i file JSON di override
    /// </summary>
    public Dictionary<string, string> DataTypes { get; set; } = new();

    /// <summary>
    /// Timestamp ultima modifica del file override
    /// </summary>
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Regole di formattazione per colonne specifiche
    /// </summary>
    public Dictionary<string, ColumnFormatRule> FormatRules { get; set; } = new();

    /// <summary>
    /// Regole di validazione per le colonne
    /// </summary>
    public Dictionary<string, ColumnValidationRule> ValidationRules { get; set; } = new();
}

/// <summary>
/// Regola di formattazione per una colonna CSV
/// </summary>
public class ColumnFormatRule
{
    public string Format { get; set; } = string.Empty;
    public string? CurrencySymbol { get; set; }
    public int DecimalPlaces { get; set; } = 2;
    public string? DateFormat { get; set; }
    public string? TrueValue { get; set; }
    public string? FalseValue { get; set; }
    public string? EmptyValue { get; set; }
    public string? NullValue { get; set; }
}

/// <summary>
/// Regola di validazione per una colonna CSV
/// </summary>
public class ColumnValidationRule
{
    public bool Required { get; set; }
    public string Type { get; set; } = string.Empty;
    public object? MinValue { get; set; }
    public object? MaxValue { get; set; }
    public int? MaxLength { get; set; }
    public string? RegexPattern { get; set; }
    public string[]? AllowedValues { get; set; }
}
