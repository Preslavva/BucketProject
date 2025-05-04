using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.DAL.Models.Entities;

namespace BucketProject.DAL.Data.InterfacesRepo
{
    public interface IGoalInviteRepo
    {
        void InsertInvitation(int goalId, int inviterId, int invitedId);
        List<GoalInvitation> GetPendingFor(int invitedId, string category);
        GoalInvitation GetById(int invitationId);
        void UpdateStatus(int invitationId, string newStatus);
        List<GoalInvitation> GetInvitationsOf(int userId, string category);
        string GetInvitationStatus(int goalId, int invitedId);
        string? GetParentGoalDescription(int subGoalId);
    }
}
