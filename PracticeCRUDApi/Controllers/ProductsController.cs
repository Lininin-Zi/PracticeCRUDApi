using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeCRUDApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PracticeCRUDApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        //依賴注入的資料庫物件
        private readonly ProductsDbContext _context;
        //建構子,依賴注入資料庫物件
        public ProductsController(ProductsDbContext context)
        {
            _context = context;
        }

        // GET: api/<ProductsController>
        [HttpGet]
        //非同步處理(async Task<ActionResult<>> 及 await)
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var Product = await _context.Products.FindAsync(id);
            //如果沒有找到就return NotFound
            if (Product == null)
            {
                return NotFound();
            }
            //有找就return OK message&data
            return Ok(Product);
        }

        // POST api/<ProductsController>
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            // 設置創建時間為當前 UTC 時間
            product.CreatedAt = DateTime.UtcNow;
            //將product加入到資料庫物件的list中
            _context.Products.Add(product);
            //實際將product寫入資料庫,以非同步方法執行,搭配await
            await _context.SaveChangesAsync();
            // 返回 201 Created 狀態碼，並在響應頭中包含新產品的 URL
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {

        }
    }
}
