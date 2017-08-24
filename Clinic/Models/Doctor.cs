using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.Models
{
    public class Doctor
    {
        public int DoctorID { get; set; }

        [Required]
        [Display(Name = "Imię")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Imię powinno mieć od 2 do 50 znaków")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Nazwisko")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Nazwisko powinno mieć od 2 do 50 znaków")]
        public string LastName { get; set; }

        [Display(Name = "Imię i nazwisko")]
        public string FullName
        {
            get
            {
                return "dr " + FirstName + " " + LastName;
            }
        }

        [Display(Name = "Notatka")]
        [DisplayFormat(NullDisplayText = "Brak notatki")]
        public string Note { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
