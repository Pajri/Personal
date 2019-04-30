using System;
using System.Collections.Generic;

namespace Personal.Models
{
    public partial class PersonalPost
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string ImageUrls { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public string UserId { get; set; }
    }
}
