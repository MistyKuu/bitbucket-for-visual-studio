using System.Collections.Generic;
using System.Linq;
using BitBucket.REST.API.Models;

namespace BitBucket.REST.API.Helpers
{
    public static class ApiHelpers
    {
        public static bool GetApproveStatus(string username, List<Participant> participants)
        {
            var approveStatus = participants.Single(x => x.Role == "REVIEWER" && x.User.Username == username).Approved;
            if (approveStatus == null) return false;
            return (bool)approveStatus;
        }
    }
}