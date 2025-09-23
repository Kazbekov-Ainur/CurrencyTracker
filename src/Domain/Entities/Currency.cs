namespace CurrencyTracker.Domain.Entities
{
    public class Currency
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Code { get; set; } = string.Empty; // USD, EUR и т.д.

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
