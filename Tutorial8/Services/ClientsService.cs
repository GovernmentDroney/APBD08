using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientsService : IClientsService
{
    private readonly string _connectionString =
        "Data Source=db-mssql;Initial Catalog=s31674;Integrated Security=True;Trust Server Certificate=True";

    public async Task<List<Client_TripDTO>> GetClient(int id)
    {
        var clientTrips = new List<Client_TripDTO>();

        string command =
            "SELECT Trip.IdTrip, Trip.Name, DateFrom, DateTo, MaxPeople, Country.Name as Country, RegisteredAt, PaymentDate " +
            "FROM dbo.Trip " +
            "JOIN dbo.Client_Trip ON Trip.IdTrip = Client_Trip.IdTrip " +
            "JOIN dbo.Country_Trip ON Trip.IdTrip = Country_Trip.IdTrip " +
            "JOIN dbo.Country ON Country_Trip.IdCountry = Country.IdCountry " +
            "WHERE Client_Trip.IdClient = @IdClient";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@IdClient", id);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idTrip = reader.GetInt32(reader.GetOrdinal("IdTrip"));

                    var existing = clientTrips.FirstOrDefault(ct => ct.Trip.Id == idTrip);

                    if (existing == null)
                    {
                        var trip = new TripDTO
                        {
                            Id = idTrip,
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                            DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                            MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                            Countries = new List<CountryDTO>()
                        };

                        var clientTrip = new Client_TripDTO
                        {
                            RegisteredAt = reader.GetInt32(reader.GetOrdinal("RegisteredAt")),
                            PaymentDate = reader.GetInt32(reader.GetOrdinal("PaymentDate")),
                            Trip = trip
                        };

                        clientTrip.Trip.Countries.Add(new CountryDTO
                        {
                            Name = reader.GetString(reader.GetOrdinal("Country"))
                        });

                        clientTrips.Add(clientTrip);
                    }
                    else
                    {
                        string countryName = reader.GetString(reader.GetOrdinal("Country"));
                        if (!existing.Trip.Countries.Any(c => c.Name == countryName))
                        {
                            existing.Trip.Countries.Add(new CountryDTO
                            {
                                Name = countryName
                            });
                        }
                    }
                }
            }
        }

        return clientTrips;
    }

    public async Task<bool> DoesClientExist(int id)
    {
        var clients = new List<int>();
        bool exists = false;
        string command = "SELECT IdClient From CLient";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@IdClient", id);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    clients.Add(reader.GetInt32(reader.GetOrdinal("IdClient")));
                }
            }

            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i] == id)
                    exists = true;
            }
        }

        return exists;
    }

    public async Task<bool> DoesTripExist(int id)
    {
        var trips = new List<int>();
        bool exists = false;
        string command = "SELECT IdTrip From Trip";


        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@IdTrip", id);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    trips.Add(reader.GetInt32(reader.GetOrdinal("IdTrip")));
                }
            }

            for (int i = 0; i < trips.Count; i++)
            {
                if (trips[i] == id)
                    exists = true;
            }
        }

        return exists;
    }

    public async Task<bool> IsTripFull(int id)
    {
        bool isThereRoom = true;
        int max = 0;
        int peopleCount = 0;
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd1 = new SqlCommand("SELECT MaxPeople FROM Trip WHERE IdTrip = @IdTrip", conn))
            {
                cmd1.Parameters.AddWithValue("@IdTrip", id);
                using (var reader = await cmd1.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        max = reader.GetInt32(reader.GetOrdinal("MaxPeople"));
                    }
                }
            }

            using (SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @IdTrip"))
            {
                cmd2.Parameters.AddWithValue("@IdTrip", id);
                peopleCount = (int)await cmd2.ExecuteScalarAsync();
            }

            if (peopleCount >= max)
            {
                isThereRoom = false;
            }
        }

        return isThereRoom;
    }

    


    public async Task<int> PostClient(ClientDTO client)
    {
        string command =
            "INSERT INTO Client VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel) SELECT @@IDENTITY";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            ;
            cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
            cmd.Parameters.AddWithValue("@LastName", client.LastName);
            cmd.Parameters.AddWithValue("@Email", client.Email);
            cmd.Parameters.AddWithValue("@Telephone", client.Telephone);
            cmd.Parameters.AddWithValue("@Pesel", client.Pesel);
            var result = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return result;
        }
    }

    public async Task PutClient(int id, int tripId)
    {
        string command = "INSERT INTO Client_Trip VALUES (@ClientId, @TripId,@RegisteredAt)";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@ClientId", id);
            cmd.Parameters.AddWithValue("@TripId", tripId);
            cmd.Parameters.AddWithValue("@RegisteredAt", DateTime.Now);
            await cmd.ExecuteScalarAsync();
        }
    }
    public async Task DeleteClient(int id, int tripId)
    {
        string command = "DELETE FROM Client_Trip WHERE IdTrip = @IdTrip AND IdClient = @IdClient";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@ClientId", id);
            cmd.Parameters.AddWithValue("@TripId", tripId);
            await cmd.ExecuteScalarAsync();
        }
    }
}