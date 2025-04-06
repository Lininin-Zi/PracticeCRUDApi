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

        //查詢所有產品資料的API
        // GET: api/<ProductsController>
        [HttpGet]
        //非同步處理(async Task<ActionResult<>> 及 await)
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }

        //查詢特定ID產品的API
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

        //新增產品的API
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

        //更新特定ID產品資料的API
        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
        {
            //檢查ID是否一致，是否需要確認ID?
            if(id != product.Id)
            {
                return BadRequest();
            }
            //檢查資料庫中是否有此ID的產品
            var existingProduct = await _context.Products.FindAsync(id);

            //如果沒有找到就return 404NotFound
            if (existingProduct == null)
            {
                return NotFound();
            }

            //更新產品資料
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.ImageUrl = product.ImageUrl;
            //更新時間戳記
            existingProduct.UpdatedAt = DateTime.UtcNow;

            //將資料更新至資料庫，並檢查是否有衝突
            //不確定這個方法是否有用，待測試
            try
            {
                // 保存更改到數據庫
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // 檢查產品是否存在，若沒有就return 404NotFound
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //return 204 no content訊息，不回傳內容
            return NoContent();
        }


        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            //檢查資料庫中是否有此ID的產品
            var existingProduct = await _context.Products.FindAsync(id);

            //如果沒有找到就return 404NotFound
            if (existingProduct == null)
            {
                return NotFound();
            }

            //將產品資料從資料庫物件list中刪除
            _context.Products.Remove(existingProduct);
            //更新至資料庫
            await _context.SaveChangesAsync();


            //return 204 no content訊息，不回傳內容
            return NoContent();
        }
        




        
        //function:用ID檢查產品是否存在於資料庫中
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
