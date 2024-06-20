using BackParroquia.Models;

namespace BackParroquia.Repositories
{
    public interface INombresRepository
    {
        Task<List<Nombres>> GetAll();
        Task<List<Nombres>> GetById(int pIdMisa);
        Task<bool> Insert(int pIdMisa, Nombres pNombres);
        Task<bool> Update(Nombres pNombres);
        Task<bool> Delete(int pIdNombre);
    }
}
