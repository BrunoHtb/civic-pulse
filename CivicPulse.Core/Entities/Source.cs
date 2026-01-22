namespace CivicPulse.Core.Entities
{
    public class Source
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Key { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string BaseUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
