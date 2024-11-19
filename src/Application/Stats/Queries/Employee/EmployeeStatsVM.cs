namespace warehouse_BE.Application.Stats.Queries.Employee;

public class EmployeeStatsVM
{
    /// <summary>
    /// Gets or sets the count of completed outbound inventory per month.
    /// </summary>
    public int OutboundInventoryCompletePerMonth { get; set; }

    /// <summary>
    /// Gets or sets the count of products nearing expiration.
    /// </summary>
    public int ProductExpirationCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of products imported per month.
    /// </summary>
    public int TotalProductImportPerMonth { get; set; }

    /// <summary>
    /// Gets or sets the total number of products exported per month.
    /// </summary>
    public int TotalProductExportPerMonth { get; set; }
}
