#nullable enable
namespace BudgetDashboard.Data.Entities;

public sealed class Budget
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public DateOnly CreatedOn { get; set; }
    public List<BudgetItem> Items { get; set; } = new();
}