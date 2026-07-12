using System.ComponentModel.DataAnnotations;
using TestApiProj.Models;

namespace TestApiProj.MainEntity
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? email { get; set; } =string.Empty;
        public string Password { get; set;}

        public string? UserRole { get; set; } = string.Empty;
    }
}
