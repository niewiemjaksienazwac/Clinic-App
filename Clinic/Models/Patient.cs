using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.Models
{
    public enum Sex
    {
        kobieta,
        mężczyzna
    }

    public class Patient
    {
        public int PatientID { get; set; }

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
                return LastName + " " + FirstName;
            }
        }

        public string FullNamePesel
        {
            get
            {
                return FullName + ", " + Pesel;
            }
        }

        [Required]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "Niepoprawny PESEL")]
        [Display(Name = "PESEL")]
        public string Pesel { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Adres e-mail")]
        public string Email { get; set; }

        [Phone]
        [Required]
        [Display(Name = "Numer telefonu")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "płeć")]
        public Sex Gender { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data urodzenia")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Notatka")]
        [DisplayFormat(NullDisplayText = "Brak notatki")]
        public string Note { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
