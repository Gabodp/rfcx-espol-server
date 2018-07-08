using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IStationRepository
    {
        Task<IEnumerable<Station>> Get();
        Task<Station> Get(string id);
        Task<Station> Get(int id);
        int GetStationCount(string apiKey);
        Task<bool> Add(Station item);
        Task<bool> Update(string id, Station item);
        Task<bool> UpdateAndroidVersion(int id, string androidV);
        Task<bool> UpdateServicesVersion(int id, string servicesV);
        Task<bool> UpdateName(int id, string name);
        Task<bool> UpdatePosition(int id, string latitud, string longitud);

        Task<bool> Remove(int id);
        Task<bool> RemoveAll();
    }
}
