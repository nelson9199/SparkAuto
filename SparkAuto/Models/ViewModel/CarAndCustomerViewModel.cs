using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SparkAuto.Models.ViewModel
{
    [NotMapped]
    public class CarAndCustomerViewModel
    {
        public ApplicationUser UserObj { get; set; }
        public List<Car> Cars { get; set; }
    }
}
