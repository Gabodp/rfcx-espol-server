using WebApplication.DbModels;
using WebApplication.IRepository;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;


namespace WebApplication.Repository
{
    public class AlertaRepository : IAlertaRepository
    {
        private readonly ObjectContext _context =null; 

        public AlertaRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 


        public async Task<IEnumerable<Alerta>> Get()
        {
            try
            {
                return await _context.Alertas.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    public async Task<Alerta> Get(string id)
    {
        var filter = Builders<Alerta>.Filter.Eq("AlertaId", id);

        try
        {
            return await _context.Alertas.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task Add(Alerta item)
    {
        try
        {
            item.Id=_context.Alertas.Find(_ => true).ToList().Count+1;
            await _context.Alertas.InsertOneAsync(item);
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
            DeleteResult actionResult = await _context.Alertas.DeleteOneAsync(
                    Builders<Alerta>.Filter.Eq("AlertaId", id));

            return actionResult.IsAcknowledged 
                && actionResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    
    public async Task<bool> Update(string id, Alerta item)
    {
        try
        {
            ReplaceOneResult actionResult 
                = await _context.Alertas
                                .ReplaceOneAsync(n => n.AlertaId.Equals(id)
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
                = await _context.Alertas.DeleteManyAsync(new BsonDocument());

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