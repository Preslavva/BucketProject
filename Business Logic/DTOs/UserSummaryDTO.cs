using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BucketProject.BLL.Business_Logic.DTOs
{
    public class UserSummaryDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] Picture { get; set; }
    }
}
