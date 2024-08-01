using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IConfiguration _configuration;

        public AuthController(IUsuarioRepositorio usuarioRepositorio, IConfiguration configuration)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            var usuarioExistente = await _usuarioRepositorio.ObtenerPorCorreo(usuario.Correo);
            if (usuarioExistente != null)
                return BadRequest("El correo ya está en uso.");

            usuario.Contraseña = BCrypt.Net.BCrypt.HashPassword(usuario.Contraseña);
            var nuevoUsuario = await _usuarioRepositorio.Crear(usuario);

            return Ok(new { nuevoUsuario.Id, nuevoUsuario.NombreUsuario, nuevoUsuario.Correo, nuevoUsuario.Rol });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var usuarioExistente = await _usuarioRepositorio.ObtenerPorCorreo(loginModel.Correo);
            if (usuarioExistente == null || !BCrypt.Net.BCrypt.Verify(loginModel.Contraseña, usuarioExistente.Contraseña))
                return Unauthorized("Correo o contraseña incorrectos.");

            var token = GenerarToken(usuarioExistente);
            return Ok(new { token });
        }

        private string GenerarToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Correo),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("usuarios")]
        public async Task<ActionResult<IEnumerable<Usuario>>> ObtenerUsuarios()
        {
            var usuarios = await _usuarioRepositorio.ObtenerTodos();
            return Ok(usuarios);
        }

        [HttpGet("usuarios/{id}")]
        public async Task<ActionResult<Usuario>> ObtenerUsuario(int id)
        {
            var usuario = await _usuarioRepositorio.ObtenerPorId(id);

            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        [HttpPut("usuarios/{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
                return BadRequest();

            await _usuarioRepositorio.Actualizar(usuario);
            return NoContent();
        }

        [HttpDelete("usuarios/{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            await _usuarioRepositorio.Eliminar(id);
            return NoContent();
        }
    }

    public class LoginModel
    {
        public string Correo { get; set; }
        public string Contraseña { get; set; }
    }
}
