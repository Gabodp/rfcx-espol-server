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
    public class DataRepository : IDataRepository
    {
        private readonly ObjectContext _context =null; 

        public DataRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 


        public async Task<IEnumerable<Data>> Get()
        {
            try
            {
                return await _context.Datas.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Data> Get(string id)
        {
            var filter = Builders<Data>.Filter.Eq("DataId", id);

            try
            {
                return await _context.Datas.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Data> Get(int id)
        {
            var filter = Builders<Data>.Filter.Eq("Id", id);

            try
            {
                return await _context.Datas.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Data>> GetByDevice(int DeviceId)
        {
            try
            {
                var filter =Builders<Data>.Filter.Eq("DeviceId", DeviceId);
                return await _context.Datas.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Data>> GetLasts()
        {
            try
            {
                List<int> dataIdList=new List<int>();
                List<Data> data;
                List<Sensor> sensor;
                var deviceCount= _context.Devices.Find(_=>true).ToList().Count;
                var sensorCount=0;
                for (int i=1;i<=deviceCount;i++){
                    var filter=Builders<Sensor>.Filter.Eq("DeviceId",i);
                    sensor=_context.Sensors.Find(filter).ToList();
                    sensorCount=sensor.Count;
                    for(int j=0;j<sensorCount;j++){
                        var filter1 =Builders<Data>.Filter.Eq("DeviceId", i) & Builders<Data>.Filter.Eq("SensorId", sensor[j].Id);
                        data=_context.Datas.Find(filter1).ToList();
                        if(data.Count>0){
                            var dataId=data.Count-1;
                            dataIdList.Add(data[dataId].Id);
                        }
                        
                    }
                }
                var filter2=Builders<Data>.Filter.In("Id", dataIdList);
                return await _context.Datas.Find(filter2).ToListAsync();
            
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Data>> GetByDeviceSensor(int DeviceId, int SensorId)
        {
            try
            {
                var filter =Builders<Data>.Filter.Eq("DeviceId", DeviceId) & Builders<Data>.Filter.Eq("SensorId", SensorId);
                return await _context.Datas.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Data>> GetByDeviceSensorTimestamp(int DeviceId, int SensorId, long StartTimestamp, long EndTimestamp)
        {
            try{
                
                var filter =Builders<Data>.Filter.Eq("DeviceId", DeviceId) & 
                Builders<Data>.Filter.Eq("SensorId", SensorId) & 
                Builders<Data>.Filter.Gte("Timestamp", StartTimestamp) &
                Builders<Data>.Filter.Lte("Timestamp", EndTimestamp);
                

                return await _context.Datas.Find(filter).ToListAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<Data> GetLastByDeviceSensor(int DeviceId, int SensorId)
        {
            
            try
            {
                int id=0;
                var filter =Builders<Data>.Filter.Eq("DeviceId", DeviceId) & Builders<Data>.Filter.Eq("SensorId", SensorId);
                List<Data> data=_context.Datas.Find(filter).ToList();
                if(data.Count>0){
                    var dataId=data.Count-1;
                    id=data[dataId].Id;
                }
                
                var filter2=Builders<Data>.Filter.Eq("Id", id);
                return await _context.Datas.Find(filter2).FirstOrDefaultAsync();
            
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Data> GetLastByDevice(int DeviceId)
        {
            
            try
            {
                int id=0;
                var filter =Builders<Data>.Filter.Eq("DeviceId", DeviceId);
                List<Data> data=_context.Datas.Find(filter).ToList();
                if(data.Count>0){
                    var dataId=data.Count-1;
                    id=data[dataId].Id;
                }
                
                var filter2=Builders<Data>.Filter.Eq("Id", id);
                return await _context.Datas.Find(filter2).FirstOrDefaultAsync();
            
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Data> Get(int DeviceId, int SensorId, int DataId)
        {
            var filter = Builders<Data>.Filter.Eq("Id", DataId) & Builders<Data>.Filter.Eq("DeviceId", DeviceId) & Builders<Data>.Filter.Eq("SensorId", SensorId);

            try
            {
                return await _context.Datas.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task Add(Data item)
        {
            try
            {
                item.Id=_context.Datas.Find(_ => true).ToList().Count+1;
                await _context.Datas.InsertOneAsync(item);
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
                DeleteResult actionResult = await _context.Datas.DeleteOneAsync(
                        Builders<Data>.Filter.Eq("DataId", id));

                return actionResult.IsAcknowledged 
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public async Task<bool> Update(string id, Data item)
        {
            try
            {
                ReplaceOneResult actionResult 
                    = await _context.Datas
                                    .ReplaceOneAsync(n => n.DataId.Equals(id)
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
                    = await _context.Datas.DeleteManyAsync(new BsonDocument());

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