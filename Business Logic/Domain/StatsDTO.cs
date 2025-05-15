using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.DAL.Models.Enums;

namespace BucketProject.BLLBusiness_Logic.Domain
{
    public class StatsDTO
    {
        public string Type { get; set; }
        public string Category { get; set; }

        public int Count { get; set; }
    }
}
