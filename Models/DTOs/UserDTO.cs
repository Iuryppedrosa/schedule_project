namespace scheduler.Models.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string FederalId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
    public class UserCreateDTO
    {
        public string FederalId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
