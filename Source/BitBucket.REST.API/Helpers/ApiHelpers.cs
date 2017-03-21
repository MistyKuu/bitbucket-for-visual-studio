using System;
using System.Collections.Generic;
using System.Linq;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Helpers
{
    public static class ApiHelpers
    {
        public static bool GetApproveStatus(string username, List<Participant> participants)
        {
            var approveStatus = participants.SingleOrDefault(x => x.Role == "REVIEWER" && x.User.Username == username)?.Approved ?? false;
            return approveStatus;
        }

        public static DateTime FromUnixTimeStamp(this long unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }

        public static long ToUnixTimeStamp(this DateTime date)
        {
            return (long)Math.Truncate((date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc))).TotalMilliseconds);
        }
    }
}