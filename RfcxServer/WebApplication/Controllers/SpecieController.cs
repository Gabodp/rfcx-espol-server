using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Net;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Encodings.Web;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using System.Diagnostics;
using Microsoft.Extensions.FileProviders;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using WebApplication.Controllers;
using WebApplication.Repository;
using System.Linq;
using System.Collections;
using Microsoft.Extensions.Primitives;
using System.Drawing;
using System.Net.Http;

namespace WebApplication
{
    [Route("api/bpv/[controller]")]
    public class SpecieController : Controller
    {
        private readonly ISpecieRepository _SpecieRepository;
        private readonly IGalleryItemRepository _GalleryItemRepository;
        private readonly IFileProvider _fileProvider;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public SpecieController(IFileProvider fileProvider, ISpecieRepository SpecieRepository, IGalleryItemRepository GalleryItemRepository)
        {
            _fileProvider = fileProvider;
            _SpecieRepository=SpecieRepository;
            _GalleryItemRepository= GalleryItemRepository;
        }

        [HttpGet("index")]
        public IActionResult Index() {
            if(TempData["creacion"] == null)
                TempData["creacion"] = 0;
            if(TempData["eliminacion"] == null)
                TempData["eliminacion"] = 0;
            if(TempData["edicion"] == null)
                TempData["edicion"] = 0;
            List<Specie> especies = _SpecieRepository.Get();
            ViewBag.especies = especies;
            return View();
        }

        [HttpGet("create")]
        public IActionResult Create() {
            return View();
        }

        [HttpGet("{id:int}/edit")]
        public IActionResult Edit(int id) {
            ViewBag.especie = _SpecieRepository.Get(id);
            DirectoryInfo DI = new DirectoryInfo(Constants.RUTA_ARCHIVOS_IMAGENES_ESPECIES + id.ToString() + "/");
            List<string> imagenes = new List<string>();
            List<string> audios = new List<string>();
            foreach (var file in DI.GetFiles())
            {
                string[] extension = (file.Name).Split('.');
                if(extension[1]!= "ogg") {
                    imagenes.Add(file.Name);
                }else{
                    audios.Add(file.Name);
                }
            }
            ViewBag.imagenes = imagenes;
            ViewBag.audios = audios;
            return View();
        }

        [HttpGet("list")]
        public List<Specie> Get()
        {
            return _SpecieRepository.Get();
        }

        private async Task<string> GetSpecie()
        {
            var Specie= _SpecieRepository.Get();
            return JsonConvert.SerializeObject(Specie);
        }

        [HttpGet("{id:int}")]
        public string Get(int id)
        {
            return this.GetSpecieByIdInt(id);
        }

        private string GetSpecieByIdInt(int id)
        {
            var Specie= _SpecieRepository.Get(id) ?? new Specie();
            return JsonConvert.SerializeObject(Specie);
        }

        [HttpGet()]
        public Task<string> Get([FromQuery] string name)
        {
            return this.GetSpecieByName(name);
        }

        private async Task<string> GetSpecieByName(string name)
        {
            var Specie= await _SpecieRepository.GetSpecie(name) ?? new Specie();
            return JsonConvert.SerializeObject(Specie);
        }


        //Utilizado para recuperar un gallery item, ya sea photo o audio, desde una carpeta
        //tipoArchivo es un string que especifica si es una image o audio. 

        //EN EL FUTURO SE PUEDE SEPARAR EN DOS O TRES REQUERIMIENTOS PARA AUDIOS IMAGENES O AUDIODESCRIPCION
        [HttpGet("{specieId:int}/gallery/{tipoArchivo}/{itemId:int}")]
        public ActionResult Get(int specieId,string tipoArchivo, int itemId)
        {
            DirectoryInfo DI = new DirectoryInfo(Constants.RUTA_ARCHIVOS_IMAGENES_ESPECIES + specieId.ToString() + "/");
            foreach (var file in DI.GetFiles())
            {
                string[] extension = (file.Name).Split('.');
                string[] numeroItem = (extension[0]).Split('_');
                //if(extension[0] == itemId.ToString()) {
                if(numeroItem.Last() == itemId.ToString()){
                    string fileAddress = DI.FullName + file.Name;
                    var net = new System.Net.WebClient();
                    var data = net.DownloadData(fileAddress);
                    var content = new System.IO.MemoryStream(data);
                    if(tipoArchivo.Equals("image")){
                        if(extension[1] == "jpg" || extension[1] == "jpeg") {
                            return File(content, "image/jpeg", file.Name);
                        }else if(tipoArchivo.Equals("image")  && (extension[1] == "png")) {
                            return File(content, "image/png", file.Name);
                        }
                    }else if(tipoArchivo.Equals("audio") && extension[1] == "ogg" ){
                        return File(content,"audio/ogg",file.Name);
                    }
                }
            }     
            return null;       
        }


        [HttpPost] //REVISAR
        public async Task<IActionResult> Post(string nombre_especie, string familia, List<string> descripciones, 
                                                List<IFormFile> archivos,List<IFormFile> audios)
                                                //AGREGADA OTRA VARIABLE QUE CONTENGA AUDIOS
        {
            string imageFilePath;
            string audioFilePath;
            Task result;

            Specie spe = new Specie();
            spe.Name = nombre_especie;
            spe.Family = familia;
            spe.Gallery = new List<GalleryItem>();
            result = _SpecieRepository.Add(spe);

            Core.MakeSpecieFolder(spe.Id.ToString()); // MODIFICAR PARA CREAR CARPETA

            //A Continuacion se guardan las imagenes y audios ingresados en el formulario
            //en su respectivo directorio 
            //Ejemplo de directorio: /var/rfcx-espol-server/resources/bpv/images/{especie(id)}/imagen(id).[jpeg/png]
            for(int i = 1; i < (archivos.Count + 1); i++)//podria ser 0 hastsa archivos.count
            {
                GalleryItem item = new GalleryItem();
                item.Description = descripciones[i - 1];

                item.audioname = nombre_especie + "_audio_descripcion_" + (i-1).ToString();
                item.imagename = nombre_especie + "_image_" + (i-1).ToString();

                _GalleryItemRepository.Add(item);
                _SpecieRepository.AddGalleryItem(spe.Id, item);

                string[] extension = (archivos[i - 1].FileName).Split('.');
                string[] extensionAudio = (audios[i-1].FileName).Split('.');
                //La siguiente linea solo trabaja con respecto al file imagenes.
                //ACTUALIZADO: Se agrego para guardar audios.
                //filePath = Path.Combine(Core.SpecieFolderPath(spe.Id.ToString()), item.Id.ToString() + "." + extension[1]);
                imageFilePath = Path.Combine(Core.SpecieFolderPath(spe.Id.ToString()), item.imagename + "." + extension[1]);
                audioFilePath = Path.Combine(Core.SpecieFolderPath(spe.Id.ToString()), item.audioname + "." + extensionAudio[1]);
                if (archivos[i - 1].Length > 0)
                {
                    using (var stream = new FileStream(imageFilePath, FileMode.Create))
                    {
                        await archivos[i - 1].CopyToAsync(stream);
                    }
                    using (var stream = new FileStream(audioFilePath, FileMode.Create))
                    {
                        await audios[i - 1].CopyToAsync(stream);
                    }
                }
            }

            if(result.Status == TaskStatus.RanToCompletion || result.Status == TaskStatus.Running ||
                result.Status == TaskStatus.Created || result.Status == TaskStatus.WaitingToRun)
                TempData["creacion"] = 1;
            else
                TempData["creacion"] = -1;           

            return Redirect("/api/bpv/specie/index/");
        }

        [HttpDelete("{id}")]
        public bool Delete(int id)
        {
            bool valor = _SpecieRepository.Remove(id);
            if(valor == true) {
                TempData["eliminacion"] = 1;
            } else {
                TempData["eliminacion"] = -1;
            }
            return true;
        }

        [HttpPatch("{id}/Name")]
        public bool PatchName(int id, [FromBody] Arrays json)
        {
            bool valor = _SpecieRepository.UpdateName(id, json.Name);
            if(valor == true) {
                TempData["edicion"] = 1;
            } else {
                TempData["edicion"] = -1;
            }
            return true;
        }

        [HttpPatch("{id}/Family")]
        public bool PatchFamily(int id, [FromBody] Arrays json)
        {
            bool valor = _SpecieRepository.UpdateFamily(id, json.Family);
            if(valor == true) {
                TempData["edicion"] = 1;
            } else {
                TempData["edicion"] = -1;
            }
            return true;
        }

        [HttpPatch("{id}/Gallery")]
        public bool PatchGallery(int id, [FromBody] Arrays json)
        {
            bool valor = _SpecieRepository.UpdateGalleryItem(id, json.IdPhoto, json.Description);
            valor = _GalleryItemRepository.UpdateDescription(json.IdPhoto, json.Description);
            if(valor == true) {
                TempData["edicion"] = 1;
            } else {
                TempData["edicion"] = -1;
            }
            return true;
        }

        [HttpPost("{id:int}/addGalleryItem")]
        public async Task<IActionResult> addGalleryItem(int id, List<string> descripciones, List<IFormFile> archivos)
        {
            string filePath;
            Task result = null;
            Specie especie = _SpecieRepository.Get(id);

            for(int i = 1; i < (archivos.Count + 1); i++)
            {
                GalleryItem item = new GalleryItem();
                item.Description = descripciones[i - 1];
                result = _GalleryItemRepository.Add(item);
                _SpecieRepository.AddGalleryItem(especie.Id, item);
                string[] extension = (archivos[i - 1].FileName).Split('.');
                filePath = Path.Combine(Core.SpecieFolderPath(especie.Id.ToString()), item.Id.ToString() + "." + extension[1]);
                if (archivos[i - 1].Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await archivos[i - 1].CopyToAsync(stream);
                    }
                }
            }

            if(result.Status == TaskStatus.RanToCompletion || result.Status == TaskStatus.Running ||
                result.Status == TaskStatus.Created || result.Status == TaskStatus.WaitingToRun)
                TempData["edicion"] = 1;
            else
                TempData["edicion"] = -1;           

            return Redirect("/api/bpv/specie/index/");
        }

    }

}