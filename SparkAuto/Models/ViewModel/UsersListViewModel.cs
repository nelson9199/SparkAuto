using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SparkAuto.Models.ViewModel
{
    [NotMapped]
    public class UsersListViewModel
    {
        public List<ApplicationUser> ApplicationUsers { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
