using BackParroquia.Models;
using Dapper;
using System.Data;

namespace BackParroquia.Repositories
{
    public class NombresRepository : INombresRepository
    {
        private readonly IDbConnection _connection;
        public NombresRepository(IDbConnection pConnection)
        {
            _connection = pConnection;
        }
        public async Task<List<Nombres>> GetAll()
        {
            string xQry = "";
            try
            {
                xQry = "SELECT * FROM Nombres " +
                "WHERE FlgEliminado != 1";
                var result = await _connection.QueryAsync<Nombres>(xQry);
                return result.ToList()!;
            }
            catch (Exception ex)
            {
                Console.WriteLine(xQry);
                Console.WriteLine($"GetAll-NombresRepository().Error-{ex.Message}");
                throw;
            }
        }
        public async Task<List<Nombres>> GetById(int pIdMisa)
        {
            string xQry = "SELECT n.IdName, n.Nombre, n.Celular FROM Nombres n " +
                "LEFT JOIN  Misas m ON n.IdMisa = m.IdMisa " +
                "WHERE m.IdMisa=@IdMisa AND n.FlgEliminado != 1";
            var result = await _connection.QueryAsync<Nombres>(xQry, param: new { IdMisa = pIdMisa });
            return result.ToList()!;
        }
        public async Task<bool> Insert(int pIdMisa, Nombres pNombres)
        {
            string xQry = "";
            try
            {
                xQry = "INSERT INTO Nombres (IdMisa, Nombre, Celular, FhCreacion,FhActualizacion,FlgEliminado) " +
                "VALUES(@IdMisa,@Name,@Number,@FhCreacion,@FhActualizacion,@FlgEliminado)";
                var result = await _connection.ExecuteAsync(xQry, new
                {
                    IdMisa = pIdMisa,
                    Name = pNombres.Nombre,
                    Number = pNombres.Celular,
                    FhCreacion = DateTime.Now,
                    FhActualizacion = DateTime.Now,
                    FlgEliminado = 0
                });
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(xQry);
                Console.WriteLine($"Insert-NombresRepository().Error-{ex.Message}");
                throw;
            }
        }
        public async Task<bool> Update(Nombres pNombres)
        {
            string xQry = "";
            try
            {
                xQry = "UPDATE Nombres SET Nombre=@Nombre, Celular=@Celular, FhActualizacion=@FhActualizacion " +
                "WHERE IdName=@IdName";
                var result = await _connection.ExecuteAsync(xQry, new
                {
                    IdName = pNombres.IdName,
                    Nombre = pNombres.Nombre,
                    Celular = pNombres.Celular,
                    FhActualizacion = DateTime.Now
                });
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(xQry);
                Console.WriteLine($"Update-NombresRepository().Error-{ex.Message}");
                throw;
            }
        }
        public async Task<bool> Delete(int pIdNombre)
        {
            string xQuery = "UPDATE Nombres SET FlgEliminado = 1 WHERE IdName = @IdNombre;";
            var result = await _connection.ExecuteAsync(xQuery, new { IdNombre = pIdNombre });
            return result > 0;
        }
    }
}
