using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeCRUDApi.Dto;
using PracticeCRUDApi.Models;
using BCrypt.Net;

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


        // GET: api/<AuthController>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerdto)
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

            // 將新使用者加入資料庫
            _context.Users.Add(user);
            // 儲存變更
            await _context.SaveChangesAsync();

            return Ok("Register");
        }

        // GET api/<AuthController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
    }
}
