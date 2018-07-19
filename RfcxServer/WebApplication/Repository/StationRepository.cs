using WebApplication.DbModels;
using WebApplication.Core;
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
using System.Text.RegularExpressions;

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
                    }s
                }
            }
            await _context.Stations.InsertOneAsync(item);
            Core.MakeStationFolder(item.Id.ToString());
            
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
            var filter4=Builders<Alert>.Filter.Eq("StationId", id);
            var filter5=Builders<AlertsConfiguration>.Filter.Eq("StationId", id);
            _context.Sensors.DeleteMany(filter1);
            _context.Datas.DeleteMany(filter2);
            _context.Audios.DeleteMany(filter3);
            _context.Audios.DeleteMany(filter4);
            _context.Audios.DeleteMany(filter5);
            Core.MakeRecyclerFolder();
            string audiosDeletedPath = Core.StationAudiosFolderPath(id.ToString());
            string audiosOggDeletedPath = Core.StationOggFolderPath(id.ToString());
            string stationDeletedPath = Core.StationFolderPath(id.ToString());
            string reclyclerPath = Core.RecyclerFolderPath();
            string audioName="";
            string recyclerName="";
            string[] filesInRecycler=Directory.GetFiles(reclyclerPath);

            if (Directory.Exists(stationDeletedPath))
            {
                string[] audios = Directory.GetFiles(stationDeletedPath);

                foreach (string audio in audios)
                {
                    audioName = Path.GetFileName(audio);
                    recyclerName = Path.Combine(reclyclerPath, audioName);
                    File.Move(audio, AutoRenameFilename(recyclerName));
                }
		Directory.Delete(stationDeletedPath);
            }

            audioName="";
            recyclerName="";

            if (Directory.Exists(audiosDeletedPath))
            {
                string[] audios1 = Directory.GetFiles(audiosDeletedPath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string audio in audios1)
                {
                    // Use static Path methods to extract only the file name from the path.
                    audioName = Path.GetFileName(audio);
                    recyclerName = Path.Combine(reclyclerPath, audioName);
                    File.Move(audio, AutoRenameFilename(recyclerName));
                }
                Directory.Delete(audiosDeletedPath);
            }

            audioName="";
            recyclerName="";

            if (Directory.Exists(audiosOggDeletedPath))
            {
                string[] audios2 = Directory.GetFiles(audiosOggDeletedPath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string audio in audios2)
                {
                    // Use static Path methods to extract only the file name from the path.
                    audioName = Path.GetFileName(audio);
                    recyclerName = Path.Combine(reclyclerPath, audioName);
                    File.Move(audio, AutoRenameFilename(recyclerName));
                }
                Directory.Delete(audiosOggDeletedPath);
                Directory.Delete(stationDeletedPath);
            }



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



    private string AutoRenameFilename(String fileCompleteName)
    {
        if (File.Exists(fileCompleteName))
    {
        string folder = Path.GetDirectoryName(fileCompleteName);
        string filename = Path.GetFileNameWithoutExtension(fileCompleteName);
        string extension = Path.GetExtension(fileCompleteName);
        int number = 1;

        Match regex = Regex.Match(fileCompleteName, @"(.+) \((\d+)\)\.\w+");

        if (regex.Success)
        {
            filename = regex.Groups[1].Value;
            number = int.Parse(regex.Groups[2].Value);
        }

        do
        {
            number++;
            fileCompleteName = Path.Combine(folder, string.Format("{0} ({1}){2}", filename, number, extension));
        }
        while (File.Exists(fileCompleteName));
    }

    return fileCompleteName;

    }


}
}
