﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Data;

public class CartCleanupService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private Timer _timer;

    public CartCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Schedule the cleanup task to run daily
        _timer = new Timer(CleanupCart, null, TimeSpan.Zero, TimeSpan.FromHours(24));
        return Task.CompletedTask;
    }

    private void CleanupCart(object state)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataDbContext>();
            var expirationDate = DateTime.UtcNow.AddDays(-7);

            var oldCarts = dbContext.CartItems.Where(c => c.Date != null && c.Date < expirationDate);
            if (oldCarts.Any())
            {
                dbContext.CartItems.RemoveRange(oldCarts);
                dbContext.SaveChanges();
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}