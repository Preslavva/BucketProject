using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BucketProject.BLLBusiness_Logic.Domain
{
    public class UserSummaryDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] Picture { get; set; }
    }
}
