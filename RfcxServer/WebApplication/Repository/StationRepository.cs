using WebApplication.DbModels;
using WebApplication.IRepository;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using MongoDB.Driver;
using System.IO;
<<<<<<< HEAD

=======
>>>>>>> 5f4350f29a2ee5d926012780d8f726b6415150a8


namespace WebApplication.Repository
{
    public class StationRepository : IStationRepository
    {
        private readonly ObjectContext _context =null; 

        public StationRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 


        public async Task<IEnumerable<Station>> Get()
        {
            try
            {
                return await _context.Stations.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    public async Task<Station> Get(string apiKey)
    {
        var filter = Builders<Station>.Filter.Eq("APIKey", apiKey);

        try
        {
            Console.WriteLine(_context.Stations.Find(filter).ToList().Count);
            if(_context.Stations.Find(filter).ToList().Count==0){
                return null;
            }
            return await _context.Stations.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public int GetStationCount(string apiKey){
        var filter = Builders<Station>.Filter.Eq("APIKey", apiKey);
        int count = _context.Stations.Find(filter).ToList().Count;
        return count;
    }

    public async Task<Station> Get(int id)
    {
        var filter = Builders<Station>.Filter.Eq("Id", id);

        try
        {
            return await _context.Stations.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> Add(Station item)
    {
        try
        {
            
            var list=_context.Stations.Find(_ => true).ToList();
            if(item.Id==0){
                if(list.Count>0){
                    list.Sort();
                    item.Id=list[list.Count-1].Id+1;
                }
                else{
                    item.Id=1;
                } 
            }else{
                for (int i=0;i<list.Count;i++){
                    if(item.Id==list[i].Id){
                        return false;
                    }
                }
            }            
            await _context.Stations.InsertOneAsync(item);
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> Remove(int id)
    {
        try
        {
            DeleteResult actionResult = await _context.Stations.DeleteOneAsync(
            Builders<Station>.Filter.Eq("Id", id));
            var filter1=Builders<Sensor>.Filter.Eq("StationId", id);
            var filter2=Builders<Data>.Filter.Eq("StationId", id);
            var filter3=Builders<Audio>.Filter.Eq("StationId", id);
            _context.Sensors.DeleteMany(filter1);
            _context.Datas.DeleteMany(filter2);
<<<<<<< HEAD
            _context.Audios.DeleteMany(filter2);
            folderStation=Core.StationFolderPath(id.ToString());
            Core.MakeRecyclerFolder();
            if (Directory.Exists(folderStation))
            {
                string[] audios = Directory.GetFiles(folderStation);
                foreach (string audio in audios)
                {
                    audioFileDeleted = Path.GetFileName(audio);
                    audioFileRecycled = Path.Combine(Core.RecyclerFolderPath(), audioFileDeleted);
                    File.Move(audioFileDeleted, audioFileRecycled);
                }

                System.IO.Directory.Delete(folderStation);
            }
            
=======
            _context.Audios.DeleteMany(filter3);

            string stationDeletedPath = Core.StationFolderPath(id.ToString());
            string reclyclerPath = Core.RecyclerFolderPath();
            string audioName="";
            string recyclerName="";

            if (Directory.Exists(stationDeletedPath))
            {
                string[] audios = Directory.GetFiles(stationDeletedPath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string audio in audios)
                {
                    // Use static Path methods to extract only the file name from the path.
                    audioName = Path.GetFileName(audio);
                    recyclerName = Path.Combine(reclyclerPath, audioName);
                    File.Move(audio, recyclerName);
                }
            }
>>>>>>> 5f4350f29a2ee5d926012780d8f726b6415150a8

            return actionResult.IsAcknowledged 
                && actionResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> Update(string id, Station item)
    {
        try
        {
            ReplaceOneResult actionResult 
                = await _context.Stations
                                .ReplaceOneAsync(n => n.StationId.Equals(id)
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
                = await _context.Stations.DeleteManyAsync(new BsonDocument());

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public Task<bool> UpdateAndroidVersion(int id, string androidV)
    {
        Station disp=getStation(id);
        disp.AndroidVersion=androidV;
        return Update(disp.StationId, disp);

    }

    public Task<bool> UpdateServicesVersion(int id, string servicesV)
    {
        Station disp=getStation(id);
        disp.ServicesVersion=servicesV;
        return Update(disp.StationId, disp);
    }

    public Task<bool> UpdateName(int id, string name)
    {
        Station disp=getStation(id);
        disp.Name=name;
        return Update(disp.StationId, disp);
    }

    public Task<bool> UpdatePosition(int id, string latitud, string longitud)
    {
        Station disp=getStation(id);
        disp.Latitude=latitud;
        disp.Longitude=longitud;
        return Update(disp.StationId, disp);        
    }

    public Station getStation(int id){
        var filter = Builders<Station>.Filter.Eq("Id", id);
        Station disp=_context.Stations.Find(filter).FirstOrDefaultAsync().Result;
        return disp;
    }


}
}