using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebFrontToBack.Models
{
    public class TeamMember
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Profession { get; set; }
        [Required]
        public string Image { get; set; }
        [Required,NotMapped]
        public IFormFile Photo { get; set; }
    }
}
