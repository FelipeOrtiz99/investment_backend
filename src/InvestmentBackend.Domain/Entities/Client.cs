using Amazon.DynamoDBv2.DataModel;

namespace InvestmentBackend.Domain.Entities;

[DynamoDBTable("Clients")]
public class Client
{
    [DynamoDBHashKey("id")]
    public string? Id { get; set; }

    [DynamoDBProperty("name")]
    public string? Name { get; set; }

    [DynamoDBProperty("state")]
    public bool State { get; set; }


    // Constructor sin parámetros para DynamoDB
    public Client() 
    {
        Id = string.Empty;
        Name = string.Empty;
        State = true;
    }

    // Constructor con parámetros para la lógica de negocio
    public Client(string id, string name, bool state)
    {
        Id = id ?? Guid.NewGuid().ToString();
        Name = name ?? string.Empty;
        State = state;
    }

    // Métodos de dominio
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        
        Name = name;
    }

    public void Activate()
    {
        State = true;
    }

    public void Deactivate()
    {
        State = false;
    }


}
