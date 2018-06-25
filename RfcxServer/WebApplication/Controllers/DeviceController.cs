using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;


namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class StationController
    {
        
        private readonly IStationRepository _StationRepository;

        public StationController(IStationRepository StationRepository)
        {
            _StationRepository=StationRepository;
        }

        [HttpGet]
        public Task<string> Get()
        {
            return this.GetStation();
        }

        public async Task<string> GetStation()
        {
            var Stations= await _StationRepository.Get();
            return JsonConvert.SerializeObject(Stations);
        }


        [HttpGet("{apiKey}")]
        public Task<string> Get(string apiKey)
        {
            return this.GetStationById(apiKey);
        }

        public async Task<string> GetStationById(string apiKey)
        {
            var Station= await _StationRepository.Get(apiKey) ?? new Station();
            return JsonConvert.SerializeObject(Station);
        }

        [HttpGet("{id:int}")]
        public Task<string> Get(int id)
        {
            return this.GetStationById(id);
        }

        public async Task<string> GetStationById(int id)
        {
            var Station= await _StationRepository.Get(id) ?? new Station();
            return JsonConvert.SerializeObject(Station);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] Station Station)
        {
            var nombre=Station.Name;
            
            await _StationRepository.Add(Station);
            return "";
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(string id, [FromBody] Station Station)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _StationRepository.Update(id, Station);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _StationRepository.Remove(id);
             
        }


        [HttpPatch("{id}/AndroidV")]
        public async Task<bool> PatchVersionAndroid(int id, [FromBody]  Arrays json)
        {
            if (id==0) return false;
            return await _StationRepository.UpdateAndroidVersion(id, json.AndroidVersion);
        }

        [HttpPatch("{id}/ServicesV")]
        public async Task<bool> PatchVersionVersionServices(int id, [FromBody]  Arrays json)
        {
            if (id==0) return false;
            return await _StationRepository.UpdateServicesVersion(id, json.ServicesVersion);
        }

        [HttpPatch("{id}/Name")]
        public async Task<bool> PatchName(int id, [FromBody]  Arrays json)
        {
            if (id==0) return false;
            return await _StationRepository.UpdateName(id, json.Name);
        }

        [HttpPatch("{id}/Coordinates")]
        public async Task<bool> PatchPosition(int id, [FromBody]  Arrays json)
        {
            if (id==0) return false;
            return await _StationRepository.UpdatePosition(id, json.Latitude, json.Longitude);
        }

    }
}