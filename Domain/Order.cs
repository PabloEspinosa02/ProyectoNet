namespace TiendaUT.Domain
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime OrderDate { get; set; }
        public List<CartItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
