using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AUEUMS.Models
{
    public class DatatableParams
    {
        public int Id { get; set; }
        public string src { get; set; }
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public string column { get; set; }
        public string dir { get; set; }
        public string searchStr { get; set; }
        public string searchBuilderLogic { get; set; }

    }
}
