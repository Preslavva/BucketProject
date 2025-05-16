using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BucketProject.BLLBusiness_Logic.Domain
{
    public class GoalInviteDTO
    {
        public int Id { get;  set; }
        public int GoalId { get;  set; }
        public int InviterId { get;  set; }
        public int InvitedId { get;  set; }
        public string Status { get;  set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
