using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ced.Data.Models
{
    [Table("Country")]
    public class Country
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Country()
        {
            Editions = new HashSet<Edition>();
            EditionCountries = new HashSet<EditionCountry>();
        }

        public int CountryId { get; set; }

        [Required]
        [StringLength(200)]
        public string CountryName { get; set; }

        [Key]
        [StringLength(3)]
        public string CountryCode { get; set; }

        [Required]
        [StringLength(2)]
        public string CountryCode2 { get; set; }

        [Required]
        [StringLength(200)]
        public string CountryLanguage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Edition> Editions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EditionCountry> EditionCountries { get; set; }
    }
}
