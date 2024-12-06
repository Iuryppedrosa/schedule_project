namespace scheduler.Models.DTOs
{
    public class EventDTO
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }

        public Guid? UserGuid { get; set; }

        public string? UserName { get; set; }
        public Guid? CourtGuid { get; set; }
        public string? Title { get; set; }

        public string? Details { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
