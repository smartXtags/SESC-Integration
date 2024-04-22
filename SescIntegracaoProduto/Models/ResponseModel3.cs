using SescIntegracaoProduto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SescIntegracaoLocal.Models
{
    internal class ResponseModel3
    {
        public bool Success { get; set; }
        public List<ConsultaItemPatrimonialModel> Data { get; set; }
    }
}
