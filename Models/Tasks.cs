using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class Tasks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(255)]
        [Display(Name = "Task Title :")]
        public string? title { get; set; }

        [Display(Name = "Task Description :")]
        public string? description { get; set; }

        [Required]
        [Display(Name = "Status :")]
        public Status status { get; set; }

        [Required]
        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }
    }

}
public enum Status
{
    InProgress = 0, Pending = 1, Completed = 2
}
