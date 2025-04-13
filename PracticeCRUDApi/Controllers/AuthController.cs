using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeCRUDApi.Dto;
using PracticeCRUDApi.Models;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PracticeCRUDApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        // 🔹 注入資料庫上下文 AppDbContext
        private readonly ProductsDbContext _context;

        // 🔹 注入 IConfiguration 來存取 appsettings.json 中的設定（如 JWT secret）
        private readonly IConfiguration _configuration;

        // 🔹 透過建構子注入這兩個服務
        public AuthController(ProductsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        // POST: api/<AuthController>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto registerdto)
        {
            // 檢查使用者名稱是否已存在
            if (await _context.Users.AnyAsync(u=>u.Username == registerdto.Username))
            {
                // 如果已存在，返回 BadRequest 提示使用者名稱已存在
                return BadRequest("Username already exists");
            }

            //檢查email是否已存在
            if (await _context.Users.AnyAsync(u=>u.Email == registerdto.Email))
            {
                // 如果已存在，返回 BadRequest 提示email已存在
                return BadRequest("Email already exists");
            }

            // 如果不存在，則創建新的使用者
            var user = new User
            {
                Username = registerdto.Username,
                Email = registerdto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerdto.Password),
                PasswordSalt = BCrypt.Net.BCrypt.GenerateSalt(),
                CreatedAt = DateTime.UtcNow,
            };

            // 將新使用者加入資料庫list物件
            _context.Users.Add(user);
            // 儲存變更
            await _context.SaveChangesAsync();

            return Ok("Register");
        }

        // POST api/<AuthController>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto logindto)
        {
            //查詢登入的User
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == logindto.Username);
            
            //帳號不存在
            if (user == null)
            {
                return Unauthorized("帳號不存在");
            }

            //驗證密碼
            bool bIsPasswordValid = BCrypt.Net.BCrypt.Verify(logindto.Password, user.PasswordHash);
            if (!bIsPasswordValid)
            {
                return Unauthorized("密碼錯誤");
            }

            //登入成功，回傳response
            var token = GenerateJwtToken(user);

            return Ok(new {token});
        }


        // POST api/<AuthController>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<AuthController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<AuthController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        //產生JWT token
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
