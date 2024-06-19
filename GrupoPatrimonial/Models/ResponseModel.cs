using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrupoPatrimonial.Models
{
    internal class ResponseModel
    {
        public bool Success { get; set; }
        public List<GrupoModel> Data { get; set; }
    }
}
