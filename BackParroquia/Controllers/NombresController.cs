using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackParroquia.Repositories;
using BackParroquia.Models;
namespace BackParroquia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NombresController : ControllerBase
    {
        private readonly INombresRepository _iNombresRepository;
        public NombresController(INombresRepository pINombresRepository)
        {
            _iNombresRepository = pINombresRepository;
        }

        [HttpGet]
        public async Task<List<Nombres>> GetAll()
        {
            var result = await _iNombresRepository.GetAll();
            return result;
        }
        [HttpGet("{pIdMisa:int}")]
        public async Task<List<Nombres>> GetById(int pIdMisa)
        {
            var result = await _iNombresRepository.GetById(pIdMisa);
            return result;
        }
        [HttpPut]
        [Route("delete")]
        public async Task<bool> Delete([FromQuery] int pId)
        {
            var result = await _iNombresRepository.Delete(pId);
            return result;
        }
    }
}
