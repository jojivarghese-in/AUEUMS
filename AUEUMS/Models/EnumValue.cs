using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AUEUMS.Models
{
    public class EnumValue
    {
        public string Name { get; set; }
        public int Value { get; set; }

    }
}
