using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IGalleryItemRepository
    {
        List<GalleryItem> Get();
        Task<GalleryItem> Get(string id);
        Task<GalleryItem> Get(int id);
        Task<bool> Add(GalleryItem item);
        bool Remove(int id);
        bool Update(int PhotoId, GalleryItem item);
        bool UpdateDescription(int id, string description);
    }

}