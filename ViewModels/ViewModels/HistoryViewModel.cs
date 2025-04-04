using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BucketProject.UI.ViewModels.ViewModels
{
    public class HistoryViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsDone { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
    }
}
