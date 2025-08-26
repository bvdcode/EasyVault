using System.ComponentModel.DataAnnotations.Schema;
using EasyExtensions.EntityFrameworkCore.Abstractions;

namespace EasyVault.Server.Database
{
    [Table("access_events")]
    public class AccessEvent : BaseEntity<Guid>
    {
        [Column("ip_address")]
        public string IpAddress { get; set; } = string.Empty;

        [Column("route")]
        public string Route { get; set; } = string.Empty;

        [Column("user_agent")]
        public string UserAgent { get; set; } = string.Empty;

        [Column("method")]
        public string Method { get; set; } = string.Empty;
    }
}
