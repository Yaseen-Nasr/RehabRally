    using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RehabRally.Web.Core.Models
{
    [Index(nameof(Name), IsUnique = true)]

    public class Category : BaseModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        //public ICollection<Exercise> Books { get; set; } = new List<Exercise>();

    }
}
