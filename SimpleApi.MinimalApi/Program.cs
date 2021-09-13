var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CustomerRepository>();

var app = builder.Build();

app.MapGet("/customers", (CustomerRepository repository) => 
{
    return repository.GetAll();
});

app.MapGet("/customers/{id}", (CustomerRepository repository, Guid id) =>
{
    var customer = repository.GetById(id);
    return customer is not null ? Results.Ok(customer) : Results.NotFound();
});

app.MapPost("/customers", (CustomerRepository repository, Customer customer) =>
{
    repository.Create(customer);
    return Results.Created($"/customers/{customer.Id}", customer);
});

app.MapDelete("/customers/{id}", (CustomerRepository repository, Guid id) =>
{
    repository.Delete(id);
    return Results.Ok();
});

app.Run();

public record Customer(Guid Id, string FullName);

public class CustomerRepository
{
    private readonly Dictionary<Guid, Customer> _customers = new();

    public void Create(Customer customer)
    {
        _customers[customer.Id] = customer;
    }

    public IEnumerable<Customer> GetAll() => _customers.Values;

    public Customer? GetById(Guid id)
    {
        if (_customers.ContainsKey(id))
            return _customers[id];

        return null;
    }

    public void Delete(Guid id)
    {
        _customers.Remove(id);
    }
}