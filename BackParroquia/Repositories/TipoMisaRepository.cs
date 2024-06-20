using BackParroquia.Models;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace BackParroquia.Repositories
{
    public class TipoMisaRepository : ITipoMisaRepository
    {
        private readonly IDbConnection _connection;
        public TipoMisaRepository(IDbConnection pConnection)
        {
            _connection = pConnection;
        }
        public async Task<List<TipoMisa>> GetAll()
        {
            string xQuery = "SELECT * FROM TipoMisa";
            var result = await _connection.QueryAsync<TipoMisa>(xQuery, new { });
            return result.ToList();
        }
    }
}
