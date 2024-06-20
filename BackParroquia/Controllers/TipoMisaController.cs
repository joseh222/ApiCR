using Microsoft.AspNetCore.Mvc;
using BackParroquia.Repositories;
using BackParroquia.Models;

namespace BackParroquia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoMisaController : ControllerBase
    {
        
        private readonly ITipoMisaRepository _iTipoMisaRepository;

        public TipoMisaController(ITipoMisaRepository pTipoMisaRepository)
        {
            _iTipoMisaRepository = pTipoMisaRepository;
        }

        [HttpGet]
        public async Task<List<TipoMisa>> GetAll()
        {
            var result = await _iTipoMisaRepository.GetAll();
            return result;
        }
    }

    
}
