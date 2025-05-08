using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<List<Client_TripDTO>> GetClient(int id);
    Task<bool> DoesClientExist(int id);
    Task <int> PostClient(ClientDTO client);
    Task PutClient(int id, int tripId);
    Task<bool> IsTripFull(int id);

    Task DeleteClient(int id, int tripId);
}