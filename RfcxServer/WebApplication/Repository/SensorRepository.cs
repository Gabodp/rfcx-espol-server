using WebApplication.IRepository;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using System.Threading.Tasks;
using WebApplication.DbModels;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using MongoDB.Driver;



namespace WebApplication.Repository
{
    public class SensorRepository : ISensorRepository
    {
        private readonly ObjectContext _context =null; 

        public SensorRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 


        public async Task<IEnumerable<Sensor>> Get()
        {
            try
            {
                return await _context.Sensors.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    public async Task<Sensor> Get(string id)
    {
        var filter = Builders<Sensor>.Filter.Eq("SensorId", id);

        try
        {
            return await _context.Sensors.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<Sensor> Get(int id)
    {
        var filter = Builders<Sensor>.Filter.Eq("Id", id);

        try
        {
            return await _context.Sensors.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<IEnumerable<Sensor>> GetByDevice(int DeviceId)
    {
        try
        {
            var filter =Builders<Sensor>.Filter.Eq("DeviceId", DeviceId);
            return await _context.Sensors.Find(filter).ToListAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<Sensor> Get(int DeviceId, int SensorId)
    {
        var filter = Builders<Sensor>.Filter.Eq("Id", SensorId) & Builders<Sensor>.Filter.Eq("DeviceId", DeviceId);

        try
        {
            return await _context.Sensors.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task Add(Sensor item)
    {
        try
        {
            item.Id=_context.Sensors.Find(_ => true).ToList().Count+1;
            await _context.Sensors.InsertOneAsync(item);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> Remove(string id)
    {
        try
        {
            DeleteResult actionResult = await _context.Sensors.DeleteOneAsync(
                    Builders<Sensor>.Filter.Eq("SensorId", id));

            return actionResult.IsAcknowledged 
                && actionResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    

    public async Task<bool> Update(string id, Sensor item)
    {
        try
        {
            ReplaceOneResult actionResult 
                = await _context.Sensors
                                .ReplaceOneAsync(n => n.SensorId.Equals(id)
                                        , item
                                        , new UpdateOptions { IsUpsert = true });
            return actionResult.IsAcknowledged
                && actionResult.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> RemoveAll()
    {
        try
        {
            DeleteResult actionResult 
                = await _context.Sensors.DeleteManyAsync(new BsonDocument());

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    }
}