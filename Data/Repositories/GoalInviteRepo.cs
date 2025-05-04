using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.DAL.Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;


namespace BucketProject.DAL.Data.Repositories
{
    public class GoalInviteRepo : Repository, IGoalInviteRepo
    {
        public GoalInviteRepo(IConfiguration configuration) : base(configuration)
        {
        }
        public void InsertInvitation(int goalId, int inviterId, int invitedId)
        {
            const string sql = @"
INSERT INTO dbo.GoalInvitation
  (GoalId, InviterId, InvitedId, Status, CreatedAt)
VALUES
  (@GoalId, @InviterId, @InvitedId, 'Pending', @CreatedAt);";

            using var conn = GetSqlConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@GoalId", goalId);
            cmd.Parameters.AddWithValue("@InviterId", inviterId);
            cmd.Parameters.AddWithValue("@InvitedId", invitedId);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

            cmd.ExecuteNonQuery();
        }

        public List<GoalInvitation> GetPendingFor(int invitedId, string category)
        {
            const string sql = @"
SELECT gi.InvitationId, gi.InviterId, gi.InvitedId, gi.GoalId, gi.Status, gi.CreatedAt
FROM dbo.GoalInvitation gi
INNER JOIN dbo.Goal g ON gi.GoalId = g.Id
WHERE gi.InvitedId = @InvitedId 
  AND gi.Status = 'Pending'
  AND g.Category = @Category
 
ORDER BY gi.CreatedAt DESC;";

            var list = new List<GoalInvitation>();
            using var conn = GetSqlConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@InvitedId", invitedId);
            cmd.Parameters.AddWithValue("@Category", category);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new GoalInvitation(
                    rdr.GetInt32(0),
                    rdr.GetInt32(1),
                    rdr.GetInt32(2),
                    rdr.GetInt32(3),
                    rdr.GetString(4),
                    rdr.GetDateTime(5)
                ));
            }

            return list;
        }


        public GoalInvitation GetById(int invitationId)
        {
            const string sql = @"
SELECT InvitationId, InviterId, InvitedId, GoalId, Status, CreatedAt
FROM dbo.GoalInvitation
WHERE InvitationId = @Id;";

            using var conn = GetSqlConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", invitationId);
            using var rdr = cmd.ExecuteReader();
            if (!rdr.Read()) return null;
            return new GoalInvitation(
      rdr.GetInt32(0),
      rdr.GetInt32(1),
      rdr.GetInt32(2),
      rdr.GetInt32(3),
      rdr.GetString(4),
      rdr.GetDateTime(5)
  );

        }

        public void UpdateStatus(int invitationId, string newStatus)
        {
            const string sql = @"
UPDATE dbo.GoalInvitation
SET Status = @Status
WHERE InvitationId = @Id;";

            using var conn = GetSqlConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", invitationId);
            cmd.Parameters.AddWithValue("@Status", newStatus);
            cmd.ExecuteNonQuery();
        }

        public List<GoalInvitation> GetInvitationsOf(int userId, string category)
        {
            const string sql = @"
SELECT
    gi.InvitationId,
    gi.InviterId,
    gi.InvitedId,
    gi.GoalId,
    gi.Status,
    gi.CreatedAt
FROM dbo.GoalInvitation AS gi
JOIN dbo.Goal           AS g  ON g.Id = gi.GoalId
WHERE gi.InviterId = @InviterId
  AND g.Category  = @Category
  AND gi.Status IN ('Pending', 'Declined')   
ORDER BY gi.CreatedAt DESC;";

            var list = new List<GoalInvitation>();
            using var conn = GetSqlConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@InviterId", userId);
            cmd.Parameters.AddWithValue("@Category", category);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new GoalInvitation(
                    rdr.GetInt32(0),
                    rdr.GetInt32(1),
                    rdr.GetInt32(2),
                    rdr.GetInt32(3),
                    rdr.GetString(4),
                    rdr.GetDateTime(5)
                ));
            }

            return list;
        }

        public string GetInvitationStatus(int goalId, int invitedId)
        {
            const string sql = @"
SELECT Status                
FROM   dbo.GoalInvitation
WHERE  GoalId    = @GoalId
  AND  InvitedId = @InvitedId
  AND  Status IN ('Pending', 'Declined');";

            using var conn = GetSqlConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@GoalId", goalId);
            cmd.Parameters.AddWithValue("@InvitedId", invitedId);  // ← fixed

            object? result = cmd.ExecuteScalar();
            return result is string s ? s : null;
        }
    }
}



