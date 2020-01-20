namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Clnd_AdUserList
    {
        [Key]
        public int UserID { get; set; }

        [StringLength(60)]
        public string UserFullName { get; set; }

        [StringLength(100)]
        public string UserLogonName { get; set; }

        [StringLength(100)]
        public string UserEmail { get; set; }

        [StringLength(100)]
        public string Pre2000LogonName { get; set; }
    }
}
