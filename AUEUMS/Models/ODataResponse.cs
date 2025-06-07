using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AUEUMS.Models
{
    public class ODataResponse<T>
    {
        public T[] Value { get; set; }

    }
}
