
using BackParroquia.Models;
using Dapper;
using System.Data;

namespace BackParroquia.Repositories
{
    public class MotivoMisaRepository : IMotivoMisaRepository
    {
        private readonly IDbConnection _connection;
        public MotivoMisaRepository(IDbConnection pConnection)
        {
            _connection = pConnection;
        }
        public async Task<List<MotivoMisa>> GetAll()
        {
            string xQuery = "SELECT * FROM MotivoMisa";
            var result = await _connection.QueryAsync<MotivoMisa>(xQuery, new { });
            return result.ToList();
        }
    }
}
