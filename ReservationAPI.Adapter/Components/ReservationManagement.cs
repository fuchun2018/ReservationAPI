using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using ReservationAPI.Ports.Entities;
using ReservationAPI.Ports.In;

namespace ReservationAPI.Adapter.Components;

public class ReservationManagement : IReservationManagement
{
    private readonly IConfiguration _config;
    
    public ReservationManagement(IConfiguration config)
    {
        this._config = config;
    }

    public async Task<IEnumerable<Doctor>> GetDoctors()
    {
        var connectionUri = _config["MongoDB:ConnectionString"];

        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);

        var client = new MongoClient(settings);
        try
        {
            //var collection = client.GetDatabase("reservation").GetCollection<Doctor>("doctors");
            //var test = (await collection.FindAsync(p=>p.Email.Equals("test@test.com"))).ToList();
            //filter doctor email = "test@test.com"
            var doctors = (await (await client.GetDatabase("reservation")
                    .GetCollection<BsonDocument>("doctors").FindAsync(new BsonDocument(){Elements = {  }})).ToListAsync())
                .Select(x => new Doctor
            {
                Name = x["name"].AsString,
                Email = x["email"].AsString
            }).ToList();

            return doctors;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return new List<Doctor>();
    }

    public async Task<IEnumerable<DateTime>> GetAvailability(string doctorEmail)
    {
        var connectionUri = _config["MongoDB:ConnectionString"];

        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);

        var client = new MongoClient(settings);

        var filter = Builders<BsonDocument>.Filter.Eq("email", doctorEmail);
        var projection = Builders<BsonDocument>.Projection.Include("availability.Date").Include("availability.Available");
        var doctors = (await client.GetDatabase("reservation")
            .GetCollection<BsonDocument>("doctors")
            .Find(filter)
            .Project(projection)
            .ToListAsync()).FirstOrDefault();

        if (doctors == null) return new List<DateTime>();

        var availability = doctors["availability"].AsBsonArray
            .Select(x => new
            {
                Date = x["Date"].ToUniversalTime().ToLocalTime(),
                Available = x["Available"].AsBoolean
            }).Where(x => x.Available).ToList();

        return availability.Select(x => x.Date);
    }
}