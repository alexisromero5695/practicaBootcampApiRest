using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace practicaBootcampApiRest.Model
{
    public class Usuario
    {
        [JsonIgnore]
        public int UsuarioPK { get; set; } // Clave primaria

        [StringLength(10, ErrorMessage = "El Rut no puede tener más de 10 caracteres.")]
        public string Rut { get; set; }
        public string Nombres { get; set; }
        public Int16 ? Edad { get; set; }
         
        //public Gustos[] GustosUsuario { get; set; }
    }

    public class Gustos
    {
        public int IdTipoMusica { get; set;}
        public string[] TipoMusica { get; set; }
    }
}
