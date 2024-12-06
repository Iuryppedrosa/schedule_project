namespace scheduler.Models.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }

        public string? Title { get; set; }
        public int? UserId { get; set; }
        public string? UserFederalId { get; set; }

        public Guid? UserGuid { get; set; }
        public string? Details { get; set; }

        public int? CourtId { get; set; }
        public Guid? CourtGuid { get; set; }

        public string? CourtName { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public User? User { get; set; }
        public Court? Court { get; set; }
    }
}
