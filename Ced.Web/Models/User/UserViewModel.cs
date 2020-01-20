namespace Ced.Web.Models.User
{
    public class UserViewModel : BaseModel
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string RoleNames { get; set; }

        public string ProfilePictureUrl { get; set; }
        
        public string EmptyProfilePictureUrl { get; set; }

        public int DraftCount { get; set; }

        public int ApprovalCount { get; set; }

        public int ApprovedCount { get; set; }

        public string Approvers{ get; set; }
    }
}