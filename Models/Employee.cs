using System.ComponentModel.DataAnnotations;

namespace EmployeeMgt.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set;}

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string Designation { get; set; }

        [MaxLength(100)]
        public string DepartmentName { get; set; }

        [MaxLength(100)]
        public string JobTitle { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        [Phone]
        public string ContactNo { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Photo { get; set; } // Store file path or URL
    }
}
