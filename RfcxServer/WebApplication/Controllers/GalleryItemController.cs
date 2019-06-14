using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;

namespace WebApplication.Controllers
{
    [Route("api/bpv/[controller]")]
    public class GalleryItemController
    {
        
        private readonly IGalleryItemRepository _GalleryItemRepository;

        public GalleryItemController(IGalleryItemRepository GalleryItemRepository)
        {
            _GalleryItemRepository=GalleryItemRepository;
        }

        [HttpGet()]
        public Task<string> Get()
        {
            return this.GetPhoto();
        }

        private async Task<string> GetPhoto()
        {
            var GalleryItem = _GalleryItemRepository.Get();
            return JsonConvert.SerializeObject(GalleryItem);
        }

        [HttpGet("{id:int}")]
        public Task<string> Get(int id)
        {
            return this.GetPhotoByIdInt(id);
        }

        private async Task<string> GetPhotoByIdInt(int id)
        {
            var GalleryItem = await _GalleryItemRepository.Get(id) ?? new GalleryItem();
            return JsonConvert.SerializeObject(GalleryItem);
        }

        [HttpDelete("{id:int}")]
        public bool Delete(int id)
        {
            bool valor = _GalleryItemRepository.Remove(id);
            return true;
        }

    }

}