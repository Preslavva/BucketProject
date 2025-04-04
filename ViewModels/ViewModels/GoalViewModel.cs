using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BucketProject.UI.ViewModels.ViewModels
{
    public class GoalViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Description { get; set; }
        public bool IsDone { get; set; }
       // public bool IsDeleted { get; set; }

      //  public List<string> AvailableTypes { get; set; } = new();
   
    }
}
