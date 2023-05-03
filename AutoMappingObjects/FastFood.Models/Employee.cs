namespace FastFood.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Employee
    {
        public Employee()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = null!;

        [Required]
        public int Age { get; set; }

        [Required]
        [MaxLength(30)]
        public string Address { get; set; } = null!;

        [ForeignKey(nameof(Position))]
        public int PositionId { get; set; }

        [Required]
        public virtual Position Position { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>(); 
    }
}