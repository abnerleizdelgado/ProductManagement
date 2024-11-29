using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Domain.DTOs;
using Domain.Interfaces;
using Domain.Entities;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IConfiguration _configuration;

        public UsersController(IUserService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] UserDTO userDTO)
        {
            if (string.IsNullOrWhiteSpace(userDTO.Username) || string.IsNullOrWhiteSpace(userDTO.Password))
                return BadRequest("Usuário e senha são obrigatórios");

            var userDb = await _service.GetByUserAsync(userDTO.Username);
            if (userDb != null)
                return BadRequest("Usuário já existe");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);

            var newUser = new User
            {
                Username = userDTO.Username,
                PasswordHash = passwordHash
            };

            await _service.AddAsync(newUser);

            return Ok(new { Message = "Usuário criado com sucesso" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            if (string.IsNullOrWhiteSpace(userDTO.Username) || string.IsNullOrWhiteSpace(userDTO.Password))
                return BadRequest("Usuário e senha são obrigatórios");

            var userDb = await _service.GetByUserAsync(userDTO.Username);
            if (userDb == null)
                return Unauthorized("Usuário ou senha inválidos");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userDTO.Password, userDb.PasswordHash);
            if (!isPasswordValid)
                return Unauthorized("Usuário ou senha inválidos");

            var claims = new[]
            {
        new Claim(ClaimTypes.Name, userDb.Username),
        new Claim(ClaimTypes.NameIdentifier, userDb.Id.ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

    }

}
