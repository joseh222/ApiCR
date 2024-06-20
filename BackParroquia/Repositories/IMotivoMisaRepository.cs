using BackParroquia.Models;

namespace BackParroquia.Repositories
{
    public interface IMotivoMisaRepository
    {
        Task<List<MotivoMisa>> GetAll();
    }
}
