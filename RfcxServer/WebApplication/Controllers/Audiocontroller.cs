using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;


namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class AudioController
    {
        
        private readonly IAudioRepository _AudioRepository;

        public AudioController(IAudioRepository AudioRepository)
        {
            _AudioRepository=AudioRepository;
        }

        [HttpGet]
        public Task<string> Get()
        {
            return this.GetAudio();
        }

        public async Task<string> GetAudio()
        {
            var Audios= await _AudioRepository.Get();
            return JsonConvert.SerializeObject(Audios);
        }


        [HttpGet]
        public Task<string> Get(string id)
        {
            return this.GetAudioById(id);
        }

        public async Task<string> GetAudioById(string id)
        {
            var Audio= await _AudioRepository.Get(id) ?? new Audio();
            return JsonConvert.SerializeObject(Audio);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] Audio Audio)
        {
            await _AudioRepository.Add(Audio);
            return "";
        }

        [HttpPut("{id}")]
        public async Task<string> Put(string id, [FromBody] Audio Audio)
        {
            if (string.isNullOrEmpty(id)) return "Id no válida";
            return await _AudioRepository.Update(id, Audio);
        }

        [HttpDelete("{id}")]
        public async Task<string> Delete(string id)
        {
            if (string.isNullOrEmpty(id)) return "Id no válida";
            await _AudioRepository.Remove(id);
            return "";
        }
    }
}