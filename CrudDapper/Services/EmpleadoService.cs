using CrudDapper.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CrudDapper.Services
{
    public class EmpleadoService
    {

        private readonly IConfiguration _configuration;
        private readonly string cadenaSql;

        public EmpleadoService(IConfiguration configuration)
        {
             _configuration = configuration;
            cadenaSql = _configuration.GetConnectionString("CadenaSQL")!;
        }

        // Obtener todos los empleados
        public async Task<List<Empleado>> Lista()
        {
            string query = "sp_listaEmpleados"; // procedure fue creado en SqlServer

            using (var conn = new SqlConnection(cadenaSql))
            {
                var lista = await conn.QueryAsync<Empleado>(query, commandType: CommandType.StoredProcedure);
                return lista.ToList();
            }
        }

        // obtener empleado por id
        public async Task<Empleado> Obtener(int id)
        {
            string query = "sp_obtenerEmpleado";
            var parametros = new DynamicParameters();
            parametros.Add("@idEmpleado", id, dbType: DbType.Int32);

            using (var conn = new SqlConnection(cadenaSql))
            {
                var empleado = await conn.QueryFirstOrDefaultAsync<Empleado>(query, parametros, commandType: CommandType.StoredProcedure);
                return empleado;
            }
        }

        // Crear empleado
        public async Task<string> Crear(Empleado objeto)
        {
            Console.WriteLine($"Número recibido en backend: {objeto.NumeroDocumento}");

            string query = "sp_crearEmpleado";
            var parametros = new DynamicParameters();

            parametros.Add("@numeroDocumento", objeto.NumeroDocumento, dbType: DbType.String);
            parametros.Add("@nombreCompleto", objeto.NombreCompleto, dbType: DbType.String);
            parametros.Add("@sueldo", objeto.Sueldo, dbType: DbType.Int32);
            parametros.Add("@msgError", dbType: DbType.String, direction: ParameterDirection.Output, size: 100);

            using (var conn = new SqlConnection(cadenaSql))
            {
                await conn.ExecuteAsync(query, parametros, commandType: CommandType.StoredProcedure);
                return parametros.Get<string>("@msgError");
            }
        }


        // Editar empleado
        public async Task<string> Editar(Empleado objeto)
        {
            string query = "sp_editarEmpleado";
            var parametros = new DynamicParameters();

            parametros.Add("@idEmpleado", objeto.IdEmpleado, dbType: DbType.Int32);
            parametros.Add("@numeroDocumento", objeto.NumeroDocumento, dbType: DbType.String);
            parametros.Add("@nombreCompleto", objeto.NombreCompleto, dbType: DbType.String);
            parametros.Add("@sueldo", objeto.Sueldo, dbType: DbType.Int32);
            parametros.Add("@msgError", dbType: DbType.String, direction: ParameterDirection.Output, size: 100);

            using (var conn = new SqlConnection(cadenaSql))
            {
                await conn.ExecuteAsync(query, parametros, commandType: CommandType.StoredProcedure);
                return parametros.Get<string>("@msgError");
            }
        }

        // Eliminar Tarea
        public async Task Eliminar(int id)
        {
            string query = "sp_eliminarEmpleado";
            var parametros = new DynamicParameters();

            parametros.Add("@idEmpleado", id, dbType: DbType.Int32);

            using (var conn = new SqlConnection(cadenaSql))
            {
                await conn.ExecuteAsync(query, parametros, commandType: CommandType.StoredProcedure);
            }
        }



    }
}
