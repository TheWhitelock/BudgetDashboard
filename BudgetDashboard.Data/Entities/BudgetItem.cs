#nullable enable
namespace BudgetDashboard.Data.Entities;

public sealed class BudgetItem
{
    public int Id { get; set; }
    public int BudgetId { get; set; }
    public Budget? Budget { get; set; }

    public string Name { get; set; } = string.Empty;
    public decimal EstimatedAmount { get; set; }
    public DateOnly CreatedOn { get; set; }
}