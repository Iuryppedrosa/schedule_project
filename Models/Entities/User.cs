namespace scheduler.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string FederalId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }

        //public virtual ICollection<User> Schedulers { get; set; }
    }
}
