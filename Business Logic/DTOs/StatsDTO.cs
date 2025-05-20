using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.DAL.Models.Enums;

namespace BucketProject.BLL.Business_Logic.DTOs
{
    public class StatsDTO
    {
        public string Type { get; set; }
        public string Category { get; set; }

        public int Count { get; set; }

        public string Ownership { get; set; }
        public int Completed { get; set; }
        public int Incomplete { get; set; }
        public string Period { get; set; }
        public int TotalGoals { get; set; }
        public int CompletedGoals { get; set; }
        public int IncompleteGoals { get; set; }
        public int PersonalGoals { get; set; }
        public int SharedGoals { get; set; }
        public int PostponedGoals { get; set; }
        public int AIGoals { get; set; }
        public int ActiveUsersCount { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }

        public int Age { get; set; }
        public string Label { get; set; }

    }
}
