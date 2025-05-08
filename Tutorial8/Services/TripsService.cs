using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString =
        "Data Source=db-mssql;Initial Catalog=s31674;Integrated Security=True;Trust Server Certificate=True";

    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

        string command =
            "SELECT Trip.IdTrip, Trip.Name, DateFrom, DateTo, MaxPeople, Country.Name as Country FROM Trip " +
            "JOIN Country_Trip ON Trip.IdTrip = Country_Trip.IdTrip " +
            "JOIN Country ON Country_Trip.IdCountry = Country.IdCountry";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {

                    // bool exists = false;
                    // int idOrdinal = reader.GetOrdinal("IdTrip");
                    // if (trips.Count != 0)
                    // {
                    //     Console.WriteLine("in if");
                    //     for (int i = 0; i < trips.Count; i++)
                    //     {
                    //         Console.WriteLine(trips[i].Id + " " + idOrdinal);
                    //         if (trips[i].Id == idOrdinal)
                    //         {
                    //             trips[i].Countries.Add(new CountryDTO()
                    //                 {
                    //                     Name = reader.GetString(reader.GetOrdinal("Country")),
                    //                 }
                    //             );
                    //             exists = true;
                    //         }
                    //     }
                    // }
                    //
                    // if (exists == false)
                    // {
                    //     trips.Add(new TripDTO()
                    //     {
                    //         Id = reader.GetInt32(idOrdinal),
                    //         Name = reader.GetString(1),
                    //         DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                    //         DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                    //         MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                    //         Countries = new List<CountryDTO>
                    //         {
                    //             new CountryDTO
                    //             {
                    //                 Name = reader.GetString(reader.GetOrdinal("Country"))
                    //             }
                    //         }
                    //     });
                    // }
                    // }
                    int idTrip = reader.GetInt32(reader.GetOrdinal("IdTrip"));
                    var trip = trips.FirstOrDefault(t => t.Id == idTrip);

                    if (trip == null)
                    {
                        trip = new TripDTO
                        {
                            Id = idTrip,
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                            DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                            MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                            Countries = new List<CountryDTO>()
                        };
                        trips.Add(trip);
                    }

                    trip.Countries.Add(new CountryDTO
                    {
                        Name = reader.GetString(reader.GetOrdinal("Country"))
                    });
                }
            }
        }

        return trips;
        }
    }