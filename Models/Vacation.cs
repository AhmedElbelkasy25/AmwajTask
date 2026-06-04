using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum VacationType
    {
        سنوية,
        عارضة,
        مرضية
    }

    public class Vacation
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = null!;
        public Employee Employee { get; set; } = null!;

        [Required]
        public string Type { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Range(1, 30, ErrorMessage = "المدة يجب أن تكون بين 1 و 30 يوم")]
        public int Duration { get; set; }

        public DateTime EndDate => StartDate.AddDays(Duration - 1);
        
    }
}
