#nullable enable
using BudgetDashboard.Data.Entities;

namespace BudgetDashboard.Data.Extensions;

/// <summary>
/// Extension methods for budget-related calculations and utilities.
/// </summary>
public static class BudgetExtensions
{
    /// <summary>
    /// Calculates the total estimated amount for all items in a budget.
    /// </summary>
    /// <param name="budget">The budget to calculate the total for.</param>
    /// <returns>The sum of all estimated amounts, or 0 if the budget has no items.</returns>
    public static decimal GetTotalAmount(this Budget budget)
    {
        return budget.Items?.Sum(i => i.EstimatedAmount) ?? 0m;
    }

    /// <summary>
    /// Calculates the total optional amount for all items in a budget.
    /// </summary>
    /// <param name="budget">The budget to calculate the optional total for.</param>
    /// <returns>The sum of all optional items' estimated amounts, or 0 if there are no optional items.</returns>
    public static decimal GetOptionalAmount(this Budget budget)
    {
        return budget.Items?.Where(i => i.IsOptional).Sum(i => i.EstimatedAmount) ?? 0m;
    }

    /// <summary>
    /// Calculates the total budgets across multiple budgets.
    /// </summary>
    /// <param name="budgets">The collection of budgets.</param>
    /// <returns>The sum of all estimated amounts across all budgets.</returns>
    public static decimal GetTotalAcrossAllBudgets(this IEnumerable<Budget> budgets)
    {
        return budgets.Sum(b => b.GetTotalAmount());
    }
}
