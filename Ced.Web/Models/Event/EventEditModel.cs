using System;
using System.ComponentModel.DataAnnotations;

namespace Ced.Web.Models.Event
{
    public class EventEditModel
    {
        [Required]
        public int EventId { get; set; }

        [Display(Name = "MasterCode", ResourceType = typeof(Resources.Resources))]
        public string MasterCode { get; set; }

        [Display(Name = "MasterName", ResourceType = typeof(Resources.Resources))]
        public string MasterName { get; set; }

        public long AxRecId { get; set; }

        [Display(Name = "Region", ResourceType = typeof(Resources.Resources))]
        public string Region { get; set; }

        [Display(Name = "Country", ResourceType = typeof(Resources.Resources))]
        public string Country { get; set; }

        [Display(Name = "City", ResourceType = typeof(Resources.Resources))]
        public string City { get; set; }

        [Display(Name = "EventType", ResourceType = typeof(Resources.Resources))]
        public string EventType { get; set; }

        [Display(Name = "Industry", ResourceType = typeof(Resources.Resources))]
        public string Industry { get; set; }

        [Display(Name = "Frequency", ResourceType = typeof(Resources.Resources))]
        public string Frequency { get; set; }

        [Display(Name = "ManagingOfficeName", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string ManagingOfficeName { get; set; }

        [Display(Name = "ManagingOfficeEmail", ResourceType = typeof(Resources.Resources))]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            ErrorMessage = @"The field Managing Office Email must be a valid email")]
        public string ManagingOfficeEmail { get; set; }

        [Display(Name = "ManagingOfficeWebsite", ResourceType = typeof(Resources.Resources))]
        [RegularExpression(@"(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?",
            ErrorMessage = @"The field Managing Office Website must be a valid url")]
        public string ManagingOfficeWebsite { get; set; }

        [Display(Name = "ManagingOfficePhone", ResourceType = typeof(Resources.Resources))]
        [RegularExpression(@"^[\+]?[1-9]{1,3}\s?[0-9]{6,11}$", ErrorMessage = @"The field Managing Office Phone must be a valid phone number")]
        public string ManagingOfficePhone { get; set; }

        [Display(Name = "DirectorEmail", ResourceType = typeof(Resources.Resources))]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string DirectorEmail { get; set; }

        [Display(Name = "LogoFileName", ResourceType = typeof(Resources.Resources))]
        public string LogoFileName { get; set; }
        
        public string LogoUrl { get; set; }

        [Display(Name = "CreateTime", ResourceType = typeof(Resources.Resources))]
        public DateTime CreateTime { get; set; }

        [Display(Name = "CreateUser", ResourceType = typeof(Resources.Resources))]
        public string CreateUser { get; set; }

        [Display(Name = "UpdateTime", ResourceType = typeof(Resources.Resources))]
        public DateTime UpdateTime { get; set; }

        [Display(Name = "UpdateUser", ResourceType = typeof(Resources.Resources))]
        public string UpdateUser { get; set; }
    }
}