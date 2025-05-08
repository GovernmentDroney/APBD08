namespace Tutorial8.Models.DTOs;

public class ClientDTO
{
    public int ClientId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string Pesel { get; set; }
}

public class Client_TripDTO
{
    public TripDTO Trip { get; set; }
    public int RegisteredAt { get; set; }
    public int? PaymentDate { get; set; }
}