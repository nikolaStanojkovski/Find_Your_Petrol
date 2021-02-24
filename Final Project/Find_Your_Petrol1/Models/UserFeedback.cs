using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Find_Your_Petrol1.Models
{
    public class UserFeedback
    {
        [Key]
        public long FeedbackId { get; set; }
        [Required]
        public string Comment { get; set; }
        public string CurrentUserUsername { get; set; }

        [Required]
        [Range(1, 5)]
        public double Rating { get; set; }

        public UserFeedback()
        {
            this.CurrentUserUsername = "";
            this.Comment = "";
            this.Rating = 0.0;
        }
    }
}