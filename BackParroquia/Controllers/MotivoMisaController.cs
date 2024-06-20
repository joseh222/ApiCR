using Microsoft.AspNetCore.Mvc;
using BackParroquia.Repositories;
using BackParroquia.Models;

namespace BackParroquia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotivoMisaController : ControllerBase
    {
        
        private readonly IMotivoMisaRepository _iMotivoMisaRepository;

        public MotivoMisaController(IMotivoMisaRepository pMotivoMisaRepository)
        {
            _iMotivoMisaRepository = pMotivoMisaRepository;
        }

        [HttpGet]
        public async Task<List<MotivoMisa>> GetAll()
        {
            var result = await _iMotivoMisaRepository.GetAll();
            return result;
        }
    }

    
}
