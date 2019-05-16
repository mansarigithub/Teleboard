namespace Teleboard.DomainModel.Core
{
    public class TenantUser
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public string UserId { get; set; }

        public virtual Tenant Tenant { get; set; }

        public virtual ApplicationUser User { get; set; }

    }
}