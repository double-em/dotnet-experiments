using GenerationTester.Interfaces;
using GenerationTester.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GenerationTester.Services;

public class DataService : IDataService
{
    private readonly ICustomersRepository _customersRepository;

    public DataService(ICustomersRepository customersRepository)
    {
        _customersRepository = customersRepository;
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IDataService, DataService>();
        services.AddTransient<ICustomersRepository, SqlCustomersRepository>();
    }

    public DataService(ICustomersRepository customersRepository) : this()
    {
        _customersRepository = customersRepository;
    }

    #region Customers

    public async Task<Customer> CreateCustomerAsync(Customer model)
    {
        return await _customersRepository.CreateAsync(model);
    }

    public async Task DeleteCustomerByIdAsync(dynamic id)
    {
        await _customersRepository.DeleteByIdAsync(id);
    }

    public async Task<List<Customer>> GetCustomersAsync()
    {
        return await _customersRepository.GetCustomerAsync();
    }

    public async Task<Customer> SaveCustomerAsync(Customer model, bool upsert = true)
    {
        return await _customersRepository.SaveAsync(model, upsert);
    }

    #endregion
}