using BackParroquia.Models;

namespace BackParroquia.Repositories
{
    public interface ITipoMisaRepository
    {
        Task<List<TipoMisa>> GetAll();
    }
}
