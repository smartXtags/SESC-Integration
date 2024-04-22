using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SescIntegracaoProduto.Models
{
    internal class ConsultaItemPatrimonialModel
    {
        public string CodigoItem { get; set; }
        public string DescricaoComplementar { get; set; }
        public string ValorDeAquisicao { get; set; }
        public string ValorDepreciado { get; set; }
        public string ValorResidual { get; set; }
    }
}
