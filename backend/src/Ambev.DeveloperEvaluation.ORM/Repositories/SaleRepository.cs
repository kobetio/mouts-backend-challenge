using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale?> GetBySaleNumberAsync(int saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        // The sale is expected to already be tracked (loaded via GetByIdAsync in this same
        // DbContext scope) and mutated through its own aggregate methods; persisting the changes
        // (including added/removed/modified items) is just a matter of saving them.
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (sale is null)
        {
            return false;
        }

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PagedResult<Sale>> ListAsync(SaleListFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include(s => s.Items)
            .AsNoTracking()
            .AsQueryable();

        if (filter.CustomerId.HasValue)
        {
            query = query.Where(s => s.Customer.Id == filter.CustomerId.Value);
        }

        if (filter.BranchId.HasValue)
        {
            query = query.Where(s => s.Branch.Id == filter.BranchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.CustomerName))
        {
            var pattern = ToLikePattern(filter.CustomerName);
            query = query.Where(s => EF.Functions.ILike(s.Customer.Name, pattern));
        }

        if (!string.IsNullOrWhiteSpace(filter.BranchName))
        {
            var pattern = ToLikePattern(filter.BranchName);
            query = query.Where(s => EF.Functions.ILike(s.Branch.Name, pattern));
        }

        if (filter.IsCancelled.HasValue)
        {
            var status = filter.IsCancelled.Value
                ? Domain.Enums.SaleStatus.Cancelled
                : Domain.Enums.SaleStatus.NotCancelled;
            query = query.Where(s => s.Status == status);
        }

        if (filter.MinTotalAmount.HasValue)
        {
            query = query.Where(s => s.TotalAmount >= filter.MinTotalAmount.Value);
        }

        if (filter.MaxTotalAmount.HasValue)
        {
            query = query.Where(s => s.TotalAmount <= filter.MaxTotalAmount.Value);
        }

        if (filter.MinSaleDate.HasValue)
        {
            query = query.Where(s => s.SaleDate >= filter.MinSaleDate.Value);
        }

        if (filter.MaxSaleDate.HasValue)
        {
            query = query.Where(s => s.SaleDate <= filter.MaxSaleDate.Value);
        }

        var orderedQuery = ApplyOrdering(query, filter.OrderBy);

        var totalCount = await orderedQuery.CountAsync(cancellationToken);
        var items = await orderedQuery
            .Skip((filter.Page - 1) * filter.Size)
            .Take(filter.Size)
            .ToListAsync(cancellationToken);

        return new PagedResult<Sale> { Items = items, TotalCount = totalCount };
    }

    /// <summary>
    /// Applies one or more "field direction" ordering clauses (comma-separated) to the query.
    /// Implemented as an explicit field switch (rather than a generic/boxed key selector) so
    /// every clause translates cleanly to SQL via the Npgsql provider.
    /// </summary>
    private static IQueryable<Sale> ApplyOrdering(IQueryable<Sale> query, string? orderBy)
    {
        var clauses = (orderBy ?? "date desc")
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        IOrderedQueryable<Sale>? ordered = null;

        foreach (var clause in clauses)
        {
            var parts = clause.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var field = parts.Length > 0 ? parts[0].ToLowerInvariant() : "date";
            var descending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            ordered = (field, descending, ordered) switch
            {
                ("salenumber", false, null) => query.OrderBy(s => s.SaleNumber),
                ("salenumber", true, null) => query.OrderByDescending(s => s.SaleNumber),
                ("salenumber", false, _) => ordered!.ThenBy(s => s.SaleNumber),
                ("salenumber", true, _) => ordered!.ThenByDescending(s => s.SaleNumber),

                ("totalamount", false, null) => query.OrderBy(s => s.TotalAmount),
                ("totalamount", true, null) => query.OrderByDescending(s => s.TotalAmount),
                ("totalamount", false, _) => ordered!.ThenBy(s => s.TotalAmount),
                ("totalamount", true, _) => ordered!.ThenByDescending(s => s.TotalAmount),

                ("status", false, null) => query.OrderBy(s => s.Status),
                ("status", true, null) => query.OrderByDescending(s => s.Status),
                ("status", false, _) => ordered!.ThenBy(s => s.Status),
                ("status", true, _) => ordered!.ThenByDescending(s => s.Status),

                ("customername", false, null) => query.OrderBy(s => s.Customer.Name),
                ("customername", true, null) => query.OrderByDescending(s => s.Customer.Name),
                ("customername", false, _) => ordered!.ThenBy(s => s.Customer.Name),
                ("customername", true, _) => ordered!.ThenByDescending(s => s.Customer.Name),

                ("branchname", false, null) => query.OrderBy(s => s.Branch.Name),
                ("branchname", true, null) => query.OrderByDescending(s => s.Branch.Name),
                ("branchname", false, _) => ordered!.ThenBy(s => s.Branch.Name),
                ("branchname", true, _) => ordered!.ThenByDescending(s => s.Branch.Name),

                (_, false, null) => query.OrderBy(s => s.SaleDate),
                (_, true, null) => query.OrderByDescending(s => s.SaleDate),
                (_, false, _) => ordered!.ThenBy(s => s.SaleDate),
                (_, true, _) => ordered!.ThenByDescending(s => s.SaleDate)
            };
        }

        return ordered ?? query.OrderByDescending(s => s.SaleDate);
    }

    /// <summary>
    /// Translates the "*" wildcard convention (§3.7) into a SQL ILIKE pattern ("%"). A value
    /// with no wildcard is treated as an exact (case-insensitive) match.
    /// </summary>
    private static string ToLikePattern(string value) => value.Contains('*') ? value.Replace('*', '%') : value;
}
