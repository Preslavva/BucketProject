using BucketProject.BLL.Business_Logic.InterfacesRepo;
using BucketProject.DAL.Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace BucketProject.DAL.Data.Repositories
{
    public class GoalInviteRepo : Repository, IGoalInviteRepo
    {
        private readonly ILogger<GoalInviteRepo> _logger;


        public GoalInviteRepo(IConfiguration configuration, ILogger<GoalInviteRepo> logger) : base(configuration)
        {
            _logger = logger;
        }
        public void InsertInvitation(int goalId, int inviterId, int invitedId)
        {
            const string sql = @"
INSERT INTO dbo.GoalInvitation
  (GoalId, InviterId, InvitedId, Status, CreatedAt)
VALUES
  (@GoalId, @InviterId, @InvitedId, 'Pending', @CreatedAt);";

            try
            {
                using var conn = GetSqlConnection();
                conn.Open();

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@GoalId", goalId);
                cmd.Parameters.AddWithValue("@InviterId", inviterId);
                cmd.Parameters.AddWithValue("@InvitedId", invitedId);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in InsertInvitation. GoalId: {GoalId}, InviterId: {InviterId}, InvitedId: {InvitedId}", goalId, inviterId, invitedId);
                throw new Exception("A database error occurred while inserting the invitation.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in InsertInvitation. GoalId: {GoalId}, InviterId: {InviterId}, InvitedId: {InvitedId}", goalId, inviterId, invitedId);
                throw;
            }
        }


        public List<GoalInvitation> GetPendingFor(int invitedId, string category)
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
INNER JOIN dbo.Goal AS g
    ON gi.GoalId = g.Id
WHERE 
    gi.InvitedId = @InvitedId
    AND gi.Status    = 'Pending'
    AND g.Category   = @Category
    AND g.IsDeleted  = 0
    AND g.Deadline > GETDATE()
ORDER BY 
    gi.CreatedAt DESC;
";

            var list = new List<GoalInvitation>();

            try
            {
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
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in GetPendingFor. InvitedId: {InvitedId}, Category: {Category}", invitedId, category);
                throw new Exception("A database error occurred while loading pending invitations.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetPendingFor. InvitedId: {InvitedId}, Category: {Category}", invitedId, category);
                throw;
            }

            return list;
        }



        public GoalInvitation GetById(int invitationId)
        {
            const string sql = @"
SELECT InvitationId, InviterId, InvitedId, GoalId, Status, CreatedAt
FROM dbo.GoalInvitation
WHERE InvitationId = @Id;";

            try
            {
                using var conn = GetSqlConnection();
                conn.Open();

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", invitationId);

                using var rdr = cmd.ExecuteReader();
                if (!rdr.Read())
                    return null;

                return new GoalInvitation(
                    rdr.GetInt32(0),
                    rdr.GetInt32(1),
                    rdr.GetInt32(2),
                    rdr.GetInt32(3),
                    rdr.GetString(4),
                    rdr.GetDateTime(5)
                );
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in GetById. InvitationId: {InvitationId}", invitationId);
                throw new Exception("A database error occurred while retrieving the invitation.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetById. InvitationId: {InvitationId}", invitationId);
                throw;
            }
        }


        public void UpdateStatus(int invitationId, string newStatus)
        {
            const string sql = @"
UPDATE dbo.GoalInvitation
SET Status = @Status
WHERE InvitationId = @Id;";

            try
            {
                using var conn = GetSqlConnection();
                conn.Open();

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", invitationId);
                cmd.Parameters.AddWithValue("@Status", newStatus);

                cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in UpdateStatus. InvitationId: {InvitationId}, NewStatus: {NewStatus}", invitationId, newStatus);
                throw new Exception("A database error occurred while updating the invitation status.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in UpdateStatus. InvitationId: {InvitationId}, NewStatus: {NewStatus}", invitationId, newStatus);
                throw;
            }
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
JOIN dbo.Goal AS g ON g.Id = gi.GoalId
WHERE gi.InviterId = @InviterId
  AND g.Category = @Category
  AND g.Deadline > GETDATE()
  AND g.IsDeleted = 0
  AND gi.Status IN ('Pending', 'Declined')
  AND NOT EXISTS (
      SELECT 1 FROM dbo.DismissedNotifications dn
      WHERE
          dn.UserId = @InviterId
          AND dn.GoalId = gi.GoalId
          AND dn.NotificationType = 'Invite'
          AND dn.TriggeredByUserId = gi.InvitedId
  )
ORDER BY gi.CreatedAt DESC;";

            var list = new List<GoalInvitation>();

            try
            {
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
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in GetInvitationsOf. UserId: {UserId}, Category: {Category}", userId, category);
                throw new Exception("A database error occurred while loading invitations.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetInvitationsOf. UserId: {UserId}, Category: {Category}", userId, category);
                throw;
            }

            return list;
        }


        public string GetInvitationStatus(int goalId, int invitedId)
        {
            const string sql = @"
SELECT Status                
FROM dbo.GoalInvitation
WHERE GoalId = @GoalId
  AND InvitedId = @InvitedId
  AND Status IN ('Pending', 'Declined');";

            try
            {
                using var conn = GetSqlConnection();
                conn.Open();

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@GoalId", goalId);
                cmd.Parameters.AddWithValue("@InvitedId", invitedId);

                object? result = cmd.ExecuteScalar();
                return result is string s ? s : null;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in GetInvitationStatus. GoalId: {GoalId}, InvitedId: {InvitedId}", goalId, invitedId);
                throw new Exception("A database error occurred while checking the invitation status.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetInvitationStatus. GoalId: {GoalId}, InvitedId: {InvitedId}", goalId, invitedId);
                throw;
            }
        }

        public string? GetParentGoalDescription(int subGoalId)
        {
            const string sql = @"
SELECT Description
FROM dbo.Goal
WHERE Id = (
    SELECT ParentGoalId
    FROM dbo.Goal
    WHERE Id = @Id
);";

            try
            {
                using var conn = GetSqlConnection();
                conn.Open();

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", subGoalId);

                var result = cmd.ExecuteScalar();
                return result as string;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in GetParentGoalDescription. SubGoalId: {SubGoalId}", subGoalId);
                throw new Exception("A database error occurred while retrieving the parent goal description.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetParentGoalDescription. SubGoalId: {SubGoalId}", subGoalId);
                throw;
            }
        }


    }
    }



