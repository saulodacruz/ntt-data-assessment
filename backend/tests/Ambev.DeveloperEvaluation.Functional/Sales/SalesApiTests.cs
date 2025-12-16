using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales;

/// <summary>
/// Functional tests for Sales API endpoints.
/// These tests verify the complete HTTP request/response flow through the API.
/// </summary>
public class SalesApiTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private static int _testCounter = 0;

    public SalesApiTests(WebApplicationFactory<Program> factory)
    {
        var testDbName = $"TestDb_{Interlocked.Increment(ref _testCounter)}";

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real database context
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DefaultContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing - each test gets its own isolated database
                services.AddDbContext<DefaultContext>(options =>
                {
                    options.UseInMemoryDatabase(testDbName);
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact(DisplayName = "POST /api/sales - Given valid sale request When creating Then should return 201 Created")]
    public async Task CreateSale_ValidRequest_ShouldReturn201Created()
    {
        // Arrange
        var request = new
        {
            SaleNumber = "S-2001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente Teste",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Centro",
            Items = new[]
            {
                new
                {
                    ProductId = Guid.NewGuid(),
                    ProductDescription = "Produto A",
                    Quantity = 5,
                    UnitPrice = 100m
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<object>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }

    [Fact(DisplayName = "GET /api/sales/{id} - Given non-existent sale id When getting Then should return 404 Not Found")]
    public async Task GetSale_NonExistentId_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/sales/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "PUT /api/sales/{id} - Given existing sale When updating Then should return 200 OK")]
    public async Task UpdateSale_ExistingSale_ShouldReturn200()
    {
        // Arrange - Create a sale first
        var createRequest = new
        {
            SaleNumber = "S-2003",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente Teste",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Centro",
            Items = new[]
            {
                new
                {
                    ProductId = Guid.NewGuid(),
                    ProductDescription = "Produto C",
                    Quantity = 8,
                    UnitPrice = 75m
                }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        var saleId = createResult!.Data!.Id;

        var updateRequest = new
        {
            // Update request body (simplified - just marking as completed)
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/sales/{saleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "POST /api/sales/{id}/cancel - Given existing sale When cancelling Then should return 200 OK")]
    public async Task CancelSale_ExistingSale_ShouldReturn200()
    {
        // Arrange - Create a sale first
        var createRequest = new
        {
            SaleNumber = "S-2004",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente Teste",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Centro",
            Items = new[]
            {
                new
                {
                    ProductId = Guid.NewGuid(),
                    ProductDescription = "Produto D",
                    Quantity = 12,
                    UnitPrice = 60m
                }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        var saleId = createResult!.Data!.Id;

        // Act
        var response = await _client.PostAsync($"/api/sales/{saleId}/cancel", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<object>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }

    [Fact(DisplayName = "POST /api/sales/{saleId}/items/{itemId}/cancel - Given existing sale and item When cancelling item Then should return 200 OK")]
    public async Task CancelSaleItem_ExistingSaleAndItem_ShouldReturn200()
    {
        // Arrange - Create a sale with items first
        var createRequest = new
        {
            SaleNumber = "S-2005",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente Teste",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Centro",
            Items = new[]
            {
                new
                {
                    ProductId = Guid.NewGuid(),
                    ProductDescription = "Produto E",
                    Quantity = 6,
                    UnitPrice = 80m
                },
                new
                {
                    ProductId = Guid.NewGuid(),
                    ProductDescription = "Produto F",
                    Quantity = 3,
                    UnitPrice = 40m
                }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        var saleId = createResult!.Data!.Id;

        // Get the sale to find item ID - we'll need to query the database directly
        // since GetSaleResponse doesn't include items in the current implementation
        // For now, we'll create a sale with a known item structure
        // In a real scenario, we'd need to either include items in GetSaleResponse
        // or query the database to get the item ID
        var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        var sale = await context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == saleId);

        if (sale == null || !sale.Items.Any())
            throw new InvalidOperationException("Sale or items not found");

        var itemId = sale.Items.First().Id;
        scope.Dispose();

        // Act
        var response = await _client.PostAsync($"/api/sales/{saleId}/items/{itemId}/cancel", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<object>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }

    [Fact(DisplayName = "DELETE /api/sales/{id} - Given existing sale When deleting Then should return 200 OK")]
    public async Task DeleteSale_ExistingSale_ShouldReturn200()
    {
        // Arrange - Create a sale first
        var createRequest = new
        {
            SaleNumber = "S-2006",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente Teste",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Centro",
            Items = new[]
            {
                new
                {
                    ProductId = Guid.NewGuid(),
                    ProductDescription = "Produto G",
                    Quantity = 7,
                    UnitPrice = 90m
                }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        var saleId = createResult!.Data!.Id;

        // Act
        var response = await _client.DeleteAsync($"/api/sales/{saleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify it was deleted
        var getResponse = await _client.GetAsync($"/api/sales/{saleId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "POST /api/sales - Given invalid request When creating Then should return 400 Bad Request")]
    public async Task CreateSale_InvalidRequest_ShouldReturn400()
    {
        // Arrange - Missing required fields
        var request = new
        {
            SaleNumber = "", // Invalid: empty
            Items = Array.Empty<object>() // Invalid: no items
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}

