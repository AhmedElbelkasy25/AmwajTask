using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Employee
    {

        [Required(ErrorMessage = "الرقم مطلوب")]
        [Display(Name = "الرقم")]
        public int Id { get; set; } 

        [Required(ErrorMessage = "الاسم مطلوب")]
        [Display(Name = "الاسم")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
        
        [Display(Name = "تاريخ الميلاد")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "المؤهل مطلوب")]
        [Display(Name = "المؤهل")]
        public int QualificationId { get; set; }
        public Qualification Qualification { get; set; } = null!;

        public int TotalVacationDays { get; set; }

        public ICollection<Vacation> Vacations { get; set; } = new List<Vacation>();
    }
}
