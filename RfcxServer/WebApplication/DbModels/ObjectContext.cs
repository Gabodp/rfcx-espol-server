using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebApplication.Models;
using System.Collections.Generic;
using WebApplication.IRepository;
using Microsoft.EntityFrameworkCore;


using System;

using System.Linq;

using System.Threading.Tasks;



namespace WebApplication.DbModels
{
    public class ObjectContext
    {
        public IConfigurationRoot Configuration { get; }
        private IMongoDatabase _database = null;

        public ObjectContext(IOptions<Settings> settings){
            Configuration=settings.Value.iConfigurationRoot;
            settings.Value.ConnectionString=Configuration.GetSection("MongoConnection:ConnectionString").Value;
            settings.Value.Database=Configuration.GetSection("MongoConnection:Database").Value;
        
            var client = new MongoClient(settings.Value.ConnectionString);
            if(client!=null){
                _database=client.GetDatabase(settings.Value.Database);
            }
        }

        public IMongoCollection<Alert> Alerts
        {
            get
            {
                return _database.GetCollection<Alert>("Alert");
            }
        }

        public IMongoCollection<AlertsConfiguration> AlertsConfigurations
        {
            get
            {
                return _database.GetCollection<AlertsConfiguration>("AlertsConfiguration");
            }
        }

        public IMongoCollection<Audio> Audios
        {
            get
            {
                return _database.GetCollection<Audio>("Audio");
            }
        }

        public IMongoCollection<Station> Stations
        {
            get
            {
                return _database.GetCollection<Station>("Station");
            }
        }

        public IMongoCollection<Specie> Species
        {
            get
            {
                return _database.GetCollection<Specie>("Specie");
            }
        }

        public IMongoCollection<GalleryItem> Photos
        {
            get
            {
                return _database.GetCollection<GalleryItem>("Photo");
            }
        }

        public IMongoCollection<Question> Questions
        {
            get
            {
                return _database.GetCollection<Question>("Question");
            }
        }

        public IMongoCollection<Label> Labels
        {
            get
            {
                return _database.GetCollection<Label>("Label");
            }
        }

        public IMongoCollection<Sensor> Sensors
        {
            get
            {
                return _database.GetCollection<Sensor>("Sensor");
            }
        }

        public IMongoCollection<Data> Datas
        {
            get
            {
                return _database.GetCollection<Data>("Data");
            }
        }
        public IMongoCollection<InfoSensores> InfoSensoress
        {
            get
            {
                return _database.GetCollection<InfoSensores>("InfoSensores");
            }
        }

        public IMongoCollection<Image> Images
        {
            get
            {
                return _database.GetCollection<Image>("Camera_Image");
                //return _database.GetCollection<Image>("image");
            }
        }

    } 
}