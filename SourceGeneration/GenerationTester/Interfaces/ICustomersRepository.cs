using GenerationTester.Models;

namespace GenerationTester.Interfaces;

public interface ICustomersRepository
{
    Task<Customer> GetByIdAsync(dynamic id);
    Task<List<Customer>> GetCustomerAsync();
    Task DeleteByIdAsync(dynamic id);
    Task<Customer> CreateAsync(Customer model);
    Task<Customer> SaveAsync(Customer model, bool upsert = true);
    Task InitDb();
}