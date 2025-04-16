using Microsoft.EntityFrameworkCore;

namespace PracticeCRUDApi.Models
{
    public class CartItem
    {
        public int Id { get; set; } // 購物車項目ID
        public int ProductId { get; set; } // 產品ID
        public int Quantity { get; set; } // 數量
        public decimal Price { get; set; } // 價格
        public int UserId { get; set; } // 使用者ID
        public virtual Product Product { get; set; } = null!; // 產品資料
        public virtual User User { get; set; } = null!; // 使用者資料
    }
}
