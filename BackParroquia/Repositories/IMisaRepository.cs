using BackParroquia.Models;

namespace BackParroquia.Repositories
{
    public interface IMisaRepository
    {
        Task<List<Misa>> GetAll();
        Task<Misa> GetById(int id);
        Task<int> GetNextId();
        Task<bool> Insert (MisaDTO misa);
        Task<bool> Update(MisaDTO misa);
        Task<bool> Delete(int pId);
        Task<bool> Print(int pId);
    }
}
