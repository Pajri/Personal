using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Personal.ViewModels
{
    public class PersonalPostViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string[] StoredImageUrls { get; set; }
        public List<IFormFile> NewImages { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public string UserId { get; set; }
    }
}
