using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using practicaBootcampApiRest.Model;
using practicaBootcampApiRest.Services;
namespace practicaBootcampApiRest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private readonly UsuarioService _usuarioService;

        // Inyección de dependencias a través del constructor
        public UsuariosController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }


        [HttpPost("createUsuario")]
        public IActionResult CreateUsuario(Usuario usuario)
        {
            string errorMessage;
            var id = _usuarioService.CreateUsuario(usuario, out errorMessage);
            switch (id)
            {
                case -1: //Error mensaje
                    return BadRequest(errorMessage);
                case > 0: //Exito
                    return CreatedAtAction(nameof(GetUsuario), new { id = id }, usuario);
                default:
                    return StatusCode(500, "Error inesperado al crear el usuario.");
            }     
        }


        [HttpPut("updateUsuario")]
        public IActionResult UpdateUsuario(int id, Usuario usuario)
        {
            string errorMessage;
            var response = _usuarioService.UpdateUsuario(id,usuario, out errorMessage);

            switch (response)
            {
                case -1: //Error mensaje
                    return BadRequest(errorMessage);                 
                case 1: //Exito
                    return CreatedAtAction(nameof(GetUsuario), new { id = id }, usuario);              
                default:
                    return StatusCode(500, "Error inesperado al crear el usuario.");               
            }
          
        }

        [HttpDelete("deleteUsuario/{id}")]
        public IActionResult DeleteUsuario(int id)
        {
            var success = _usuarioService.DeleteUsuario(id);
            if (success)
            {
                return Ok("Eliminado con éxito");
            }
            return NotFound("Usuario no encontrado");
        }


        [HttpGet("getUsuario/{id}")]
        public IActionResult GetUsuario(int id)
        {
            var usuario = _usuarioService.GetUsuario(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }


        [HttpGet("getUsuarios")]
        public IActionResult GetUsuarios()
        {
            var usuarios = _usuarioService.GetUsuarios();
            if (usuarios.Any())
            {
                return Ok(usuarios);
            }
            return NotFound("No hay usuarios disponibles.");
        }

    }
}
