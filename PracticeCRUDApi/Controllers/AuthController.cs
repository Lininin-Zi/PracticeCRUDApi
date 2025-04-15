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
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PracticeCRUDApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        //注入資料庫上下文 AppDbContext
        private readonly ProductsDbContext _context;

        //注入 IConfiguration 來存取 appsettings.json中的設定(JWT）
        private readonly IConfiguration _configuration;

        //透過建構子依賴注入
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

        [Authorize]
        // GET api/<AuthController>
        [HttpGet("me")]
        public async Task<IActionResult> GetMyInfo()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Token does not contain a valid user ID.");
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("Invalid user ID format.");
            }

            //從資料庫撈該使用者的資料
            var user = await _context.Users.FindAsync(userId);

            //若找不到，回傳 404
            if (user == null)
                return NotFound();

            //回傳資料，避免包含敏感資訊（像密碼）
            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email
            });
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserDto updateuserdto)
        {
            //從 Token 取出 UserId
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            //從資料庫撈出該會員資料
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            //更新 Email
            user.Email = updateuserdto.Email;

            //若有輸入新密碼，就進行加密更新
            if (!string.IsNullOrEmpty(updateuserdto.NewPassword))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateuserdto.NewPassword);
            }

            //存回資料庫
            await _context.SaveChangesAsync();

            //回傳 204NoContent 代表成功但無內容回應
            return NoContent();
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
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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
