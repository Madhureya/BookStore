using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }


        [Required]
        [StringLength(100)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Zoner { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public string ReleaseDate { get; set; }

        [Required]
        
        public int Cost { get; set; }

        [Display(Name = "image")]

        public string? bookimg { get; set; }

    }
}
