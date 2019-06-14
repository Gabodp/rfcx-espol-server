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
using System.Text.RegularExpressions;
using System.Net;

namespace WebApplication.Repository
{
    public class SpecieRepository : ISpecieRepository
    {
        private readonly ObjectContext _context =null; 

        public SpecieRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 

        public List<Specie> Get()
        {
            try
            {
                return _context.Species.Find(_ => true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Specie> Get(string id)
        {
            var filter = Builders<Specie>.Filter.Eq("SpecieId", id);

            try
            {
                return await _context.Species.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Specie> GetSpecie(string name)
        {
            var filter = Builders<Specie>.Filter.Eq("Name", name);

            try
            {
                return await _context.Species.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Specie Get(int id)
        {
            var filter = Builders<Specie>.Filter.Eq("Id", id);

            try
            {
                return _context.Species.Find(filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Add(Specie item)
        {
            try
            {
                var list=_context.Species.Find(_ => true).ToList();
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
    
                await _context.Species.InsertOneAsync(item);
                Core.MakeSpecieFolder(item.Id.ToString());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Update(int SpecieId, Specie item)
        {
            try
            {
                ReplaceOneResult actionResult 
                    = _context.Species
                                    .ReplaceOne(n => n.Id.Equals(SpecieId)
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

        public bool UpdateName(int id, string name)
        {
            var filter = Builders<Specie>.Filter.Eq("Id", id);
            Specie specie  = _context.Species.Find(filter).FirstOrDefault();
            specie.Name = name;
            return Update(id, specie);
        }

        public bool UpdateFamily(int id, string family)
        {
            var filter = Builders<Specie>.Filter.Eq("Id", id);
            Specie specie  = _context.Species.Find(filter).FirstOrDefault();
            specie.Family = family;
            return Update(id, specie);
        }

        public bool UpdateGallery(int id, List<GalleryItem> gallery)
        {
            var filter = Builders<Specie>.Filter.Eq("Id", id);
            Specie specie  = _context.Species.Find(filter).FirstOrDefault();
            specie.Gallery = gallery;
            return Update(id, specie);
        }

        public bool UpdateGalleryItem(int id, int id_gallery_item, string _description)//ACTUALIZA SOLO DESCRIPCION
        {
            var filter = Builders<Specie>.Filter.Eq("Id", id);
            Specie specie  = _context.Species.Find(filter).FirstOrDefault();
            int indice_item_galeria = specie.Gallery.FindIndex(f => f.Id == id_gallery_item);
            specie.Gallery[indice_item_galeria].Description = _description;
            return Update(id, specie);
        }

        public bool AddGalleryItem(int specieId, GalleryItem item)
        {
            Specie specie = getSpecie(specieId);
            specie.Gallery.Add(item);
            return Update(specie.Id, specie);
        }

        public Specie getSpecie(int id){
            var filter = Builders<Specie>.Filter.Eq("Id", id);
            Specie specie=_context.Species.Find(filter).FirstOrDefaultAsync().Result;
            return specie;
        }

        public bool Remove(int id)
        {
            try
            {
                var filtro_especie = Builders<Specie>.Filter.Eq("Id", id);
                Specie specie = _context.Species.Find(filtro_especie).FirstOrDefault();
                foreach(GalleryItem item in specie.Gallery) {
                    var filtro_item_galeria = Builders<GalleryItem>.Filter.Eq("Id", item.Id);
                    _context.Photos.DeleteOne(filtro_item_galeria); //.Photos se refiere a GalleryItems en la base de datos
                }
                DeleteResult actionResult = _context.Species.DeleteOne(filtro_especie);
                var filtro_pregunta = Builders<Question>.Filter.Eq("SpecieId", id);
                _context.Questions.DeleteMany(filtro_pregunta);
                string specieDeletedPath = Core.SpecieFolderPath(id.ToString());
                Directory.Delete(specieDeletedPath, true);              
                return actionResult.IsAcknowledged 
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.Write("error: " + ex.StackTrace + "\n");
                Console.Write("error: " + ex.Message + "\n");
                throw ex;
            }
        }

    }

}