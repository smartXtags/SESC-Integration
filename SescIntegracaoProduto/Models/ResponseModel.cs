using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SescIntegracaoLocal.Models
{
    internal class ResponseModel
    {
        public bool Success { get; set; }
        public List<ProdutoModel> Data { get; set; }
        public List<string> Message { get; set; }
    }
}
