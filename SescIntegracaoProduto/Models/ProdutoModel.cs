using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SescIntegracaoLocal.Models
{
    internal class ProdutoModel
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string DatadeAquisição { get; set; }
        public string GrupoPatrimonial { get; set; }
        public string Local { get; set; }
        public string Datadabaixa { get; set; }
        public string DataHoraAlteracaoHistorico { get; set; }
        public string Status { get; set; }
        public string Disposicao { get; set; }
        public string DescricaoComplementar { get; set; }
        public string ValorDeAquisicao { get; set; }
        public string ValorDepreciado { get; set; }
        public string ValorResidual { get; set; }
        public string Departamento { get; set; }
    }
}
