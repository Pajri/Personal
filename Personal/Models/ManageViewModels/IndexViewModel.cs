using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Personal.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public ApplicationUser User { get; set; }
        public string StatusMessage { get; set; }
    }
}
