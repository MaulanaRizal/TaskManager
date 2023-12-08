using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class Task
    {

        [Key]
        public int id { get; set; }
        [Required]
        [MaxLength(255)]
        public string? title { get; set; }
        [Required]
        public string? description { get; set; }
        public Status status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}



public enum Status
{
    Pending, Inprogres, Compeled
}