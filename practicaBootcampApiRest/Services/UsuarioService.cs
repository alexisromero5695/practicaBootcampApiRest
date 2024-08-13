using practicaBootcampApiRest.Model;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;


namespace practicaBootcampApiRest.Services
{
    public class UsuarioService
    {
        private readonly string _connectionString;

        public UsuarioService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public IDbConnection Connection => new MySqlConnection(_connectionString);


        private bool ValidarRUT(string rutCompleto)
        {
            rutCompleto = rutCompleto.Replace(".", "").Replace("-", "").ToUpper();

            if (rutCompleto.Length < 8 || rutCompleto.Length > 9)
                return false;

            string rut = rutCompleto.Length == 9 ? rutCompleto.Substring(0, 8) : rutCompleto.Substring(0, 7);
            char dv = rutCompleto[^1];

            int suma = 0;
            int multiplicador = 2;

            for (int i = rut.Length - 1; i >= 0; i--)
            {
                suma += (rut[i] - '0') * multiplicador;
                multiplicador++;
                if (multiplicador > 7)
                    multiplicador = 2;
            }

            int dvCalculado = 11 - (suma % 11);
            if (dvCalculado == 11) dvCalculado = 0;
            if (dvCalculado == 10) dvCalculado = 'K';
            else dvCalculado = (char)('0' + dvCalculado);

            return dv == dvCalculado;
        }

        private string FormatearRUT(string rut)
        {
            rut = rut.Replace(".", "").Replace("-", "").ToUpper();       
            if (rut.Length < 8 || rut.Length > 9)
                return rut; 
            string rutBase = rut.Length == 9 ? rut.Substring(0, 8) : rut.Substring(0, 7);
            string dv = rut.Length == 9 ? rut.Substring(8, 1) : rut.Substring(7, 1);          
            return $"{rutBase}-{dv}";
        }


        public int CreateUsuario(Usuario usuario, out string errorMessage)

        {

            if (!ValidarRUT(usuario.Rut))
            {
                errorMessage = "El RUT ingresado no es válido.";
                return -1;
            }
            usuario.Rut = FormatearRUT(usuario.Rut);
            using (var connection = Connection)
            {  
                string checkSql = "SELECT COUNT(1) FROM usuario WHERE Rut = @Rut";
                int existingCount = connection.ExecuteScalar<int>(checkSql, new { Rut = usuario.Rut });

                if (existingCount > 0)
                {
                    errorMessage = "El Rut ya existe en la base de datos.";
                    return -1;
                }

                string sql = @"INSERT INTO usuario (Rut, Nombres, Edad) 
                       VALUES (@Rut, @Nombres, @Edad);
                       SELECT LAST_INSERT_ID();";
     
                var id = connection.QuerySingle<int>(sql, usuario);
                errorMessage = null;
                return id;
            }
        }

        public int UpdateUsuario(int id, Usuario usuario, out string errorMessage)
        {

            if (!ValidarRUT(usuario.Rut))
            {
                errorMessage = "El RUT ingresado no es válido.";
                return -1;
            }

            using (var connection = Connection)
            {     
                string checkSql = "SELECT COUNT(1) FROM usuario WHERE Rut = @Rut" +
                    " and@UsuarioPK != @UsuarioPK";
                usuario.Rut = FormatearRUT(usuario.Rut);
                int existingCount = connection.ExecuteScalar<int>(checkSql, new {  UsuarioPK = id, Rut = usuario.Rut });

                if (existingCount > 0)
                {                
                    errorMessage = "El Rut ya existe en la base de datos.";
                    return -1;
                }

                string sql = @"UPDATE usuario 
                       SET Rut = @Rut, Nombres = @Nombres, Edad = @Edad 
                       WHERE UsuarioPK = @UsuarioPK";
                                
                int rowsAffected = connection.Execute(sql, new
                {
                    Rut = usuario.Rut,
                    Nombres = usuario.Nombres,
                    Edad = usuario.Edad,
                    UsuarioPK = id
                });           
                if(rowsAffected == 0)
                {
                    errorMessage = "Usuario no encontrado";
                    return -1;
                }
                errorMessage = null;
                return 1;
            }
        }


        public Usuario GetUsuario(int id)
        {
            using (var connection = Connection)
            {
                string sql = "SELECT * FROM usuario WHERE UsuarioPK = @Id";
                return connection.QueryFirstOrDefault<Usuario>(sql,new { Id = id });     
            }
        }

        public IEnumerable<Usuario> GetUsuarios()
        {
            using (var connection = Connection)
            {
                string sql = "SELECT * FROM usuario";
                return connection.Query<Usuario>(sql);
            }
        }

        public bool DeleteUsuario(int id)
        {
            using (var connection = Connection)
            {
                string sql = "DELETE FROM usuario WHERE UsuarioPK = @Id";

                int rowsAffected = connection.Execute(sql, new { Id = id });
                return rowsAffected > 0;
            }
        }

    }

   
}
