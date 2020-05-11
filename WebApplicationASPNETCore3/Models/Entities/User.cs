using System;
using System.Collections.Generic;

namespace CRM.Models.Entities
{
    public partial class User
    {
        public User()
        {
            UserPermission = new HashSet<UserPermission>();
            CampaignTeam = new HashSet<CampaignTeam>();
            CampaignTeamMember = new HashSet<CampaignTeamMember>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? LastWKCampaignId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public virtual UserCredential UserCredential { get; set; }
        public virtual ICollection<UserPermission> UserPermission { get; set; }
        public virtual ICollection<CampaignTeam> CampaignTeam { get; set; }

        public virtual ICollection<CampaignTeamMember> CampaignTeamMember { get; set; }
    }
}
