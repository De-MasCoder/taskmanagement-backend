using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace TaskContracts.Models
{
    [Table("Users")]
    public class User: BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }
        [Column("name")]
        public  string Name { get; set; }
        [Column("email_address")]
        public  string EmailAddress { get; set; }
        [Column("last_name")]
        public string? LastName { get; set; }
        [Column("occupation")]
        public string? Occupation { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("role")]
        public  string Role { get; set; }

    }
}
