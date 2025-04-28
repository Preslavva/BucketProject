using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BucketProject.DAL.Models.Entities
{
    public class GoalInvitation
    {
        public int Id { get; private set; }
        public int GoalId { get; private set; }
        public int InviterId { get; private set; }
        public int InvitedId { get; private set; }
        public string Status { get; private set; }    
        public DateTime CreatedAt { get; private set; }

        public GoalInvitation(int id, int inviterId, int invitedId, int goalId, string status, DateTime createdAt)
        {
            Id = id;
            InviterId = inviterId;
            InvitedId = invitedId;
            GoalId = goalId;
            Status = status;
            CreatedAt = createdAt;
                
        }
    }
}
