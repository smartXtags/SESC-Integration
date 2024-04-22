using Produto.Models;
using SescIntegracaoLocal.Configs;
using SescIntegracaoProduto.Models;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.Serialization;

HttpClient client = new HttpClient();
ApplicationPropperts properts = new ApplicationPropperts();
LogModel log = new LogModel();

client.Timeout = TimeSpan.FromMinutes(100);

var request = new XDocument(
    new XElement("msg",
        new XElement("command", "GetObject"),
        new XElement("terminal", "ERP"))
    );

var request2 = new XDocument(
    new XElement("msg",
        new XElement("command", "GetLocation"),
        new XElement("terminal", "ERP"))
    );

try
{
    var response = await client.PostAsync(properts.ApiXtrack(),
        new StringContent(request.ToString(), System.Text.Encoding.UTF8, "application/xml"));

    var response2 = await client.PostAsync(properts.ApiXtrack(),
        new StringContent(request2.ToString(), System.Text.Encoding.UTF8, "application/xml"));


    string resultXml = await response.Content.ReadAsStringAsync();
    
    string resultXml2 = await response2.Content.ReadAsStringAsync();

    if (resultXml == null || resultXml == "" && resultXml2 == null || resultXml2 == "")
    {
        log.SaveLogToFile(Path.Combine(properts.LogPath(),
                        $"log_ProdutosOUT {DateTime.Now:yyyy-MM-dd}.txt"),
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} - Error: {response.RequestMessage}");
        log.SaveLogToFile(Path.Combine(properts.LogPath(),
                        $"log_ProdutosOUT {DateTime.Now:yyyy-MM-dd}.txt"),
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} - Error: {response2.RequestMessage}");
    }
    else
    {

        log.SaveLogToFile(Path.Combine(properts.LogPath(),
                        $"log_ProdutosOUT {DateTime.Now:yyyy-MM-dd}.txt"),
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} - Success: {response.RequestMessage}");

        log.SaveLogToFile(Path.Combine(properts.LogPath(),
                        $"log_ProdutosOUT {DateTime.Now:yyyy-MM-dd}.txt"),
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} - Success: {response2.RequestMessage}");


        var doc = XDocument.Parse(resultXml);
        var xmlSerializer = new XmlSerializer(typeof(DocumentElement));
        var data = (DocumentElement)xmlSerializer.Deserialize(doc.CreateReader());

        var doc2 = XDocument.Parse(resultXml2);
        var xmlSerializer2 = new XmlSerializer(typeof(DocumentElement2));
        var data2 = (DocumentElement2)xmlSerializer2.Deserialize(doc2.CreateReader());

        
        int batchSize = 1000;
        for (int i = 0; i <= data.Data.Count; i += batchSize)
        {
            int count = Math.Min(batchSize, data.Data.Count - i);
            var batch = data.Data.GetRange(i, count);


            foreach (var product in batch)
            {
                var location = data2.LocationModels.FirstOrDefault(l => l.ID == product.LOCATION_ID);

                if (location != null)
                {
                    product.LOCATION_ID = location.USR3;
                }
            }
            foreach (var item in batch)
            {
                item.USR1.Substring(0, 10);
                item.USR1.Replace("/", "");
                if (item.ACTIVE == "0" || item.ACTIVE == "10")
                {
                    item.ACTIVE = DateTime.Now.ToString("ddMMyyyy");
                }
                else
                {
                    item.ACTIVE = "";
                }
            }

            List<object> list = new List<object>();
            foreach (var item in batch)
            {
                var patrimonio = new
                {
                    CodigoEmpresa = "PE",
                    CodigoPatrimonio = item.IDCODE,
                    CodigoAnexo = "000",
                    DescricaoDoBem = item.DESCRIPTION,
                    CodigoFornecedorDoBem = "",
                    NomeFornecedorDoBem = "",
                    NumerodoDocumento = "",
                    DataAquisicaoDoBem = item.USR1.Substring(0, 10).Replace("/", ""),
                    LancamentoContabildaAquisicao = "",
                    CodigoGrupoPatrimonialDoBem = "",
                    CodigoCentrodeCusto = "",
                    LocalOndeoBemEstaAlocado = item.LOCATION_ID,
                    NomedoUsuariodoBem = "",
                    NumerodeSerie = "",
                    CodigoItemEstoque = "",
                    DataqueResponsavelFezoCadastro = "",
                    DatadaVendaoudoBem = "",
                    MotivodaBaixa = "",
                    ValordaBaixa = "",
                    LancamentoContabildeBaixa = "",
                    Inventario = "",
                    DatadeAberturadoInventario = "",
                    ValordeAquisicaodoBem = item.USR6,
                    ValorDespesasAcessorias = "",
                    DatadeEntradaemUsodoBem = "",
                    PercentualDepreciacaodoBem = "",
                    ValorDepreciacaoAcumuladadoItem = "",
                    DepreciacaoValorAcrescimoDecrescimo = "",
                    DataUltimoMovimento = "",
                    DataUsoSistema = "",
                    SubgrupoPatrimonial = "",
                    CodigoFilial = "",
                    DescricaoComplementar = item.USR5,
                    Quantidade = item.QUANTITY,
                    QuantidadeAcrescimoDecrescimo = "",
                    DatadeGarantia = "",
                    ItemGeraCIAP = "",
                    ValorMaximoDepreciacao1 = "1,00",
                    CodigodoProjeto = "",
                    TipoOperacao = "A",
                    InterfaceClassificacaoEspecificaPatrimonio = new List<object>
                    {
                        new
                            {
                            Tipo = "P",
                            Classe = "0001",
                            Valor = "ST02"
                        }
                    }
                };

                list.Add(patrimonio);
            }


            var requestJson = new
                {
                    AutheticationToken = new
                    {
                        Username = "INTEGRACAOPE",
                        Password = "Fpjuu4fj_-04e1vj",
                        EnvironmentName = "SESCPEHOM"
                    },
                    Data = new
                    {
                        InterfacedoPatrimonio = list
                    }
                };

                try
                {
                    var requestJsonContent = JsonSerializer.Serialize(requestJson);
                    var responseRequestJson = await client.PostAsync(properts.ApiMXM(),
                        new StringContent(requestJsonContent, System.Text.Encoding.UTF8, "application/json"));

                    string resultMXM = await responseRequestJson.Content.ReadAsStringAsync();

                    if (resultMXM == null || resultMXM == "" || resultMXM == "\"{\\\"Message\\\":\\\"An error has occurred.\\\"}\"")
                    {
                        log.SaveLogToFile(Path.Combine(properts.LogPath(),
                        $"log_ProdutosOUT {DateTime.Now:yyyy-MM-dd}.txt"),
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} - Error: {responseRequestJson}");
                    }
                    else
                    {
                        log.SaveLogToFile(Path.Combine(properts.LogPath(),
                        $"log_ProdutosOUT {DateTime.Now:yyyy-MM-dd}.txt"),
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} - Success: {resultMXM}");
                    }

                }
                catch (Exception ex)
                {
                    log.SaveLogToFile(Path.Combine(properts.LogPath(),
                    $"log_ProdutosOUT {DateTime.Now:yyyy-MM-dd}.txt"),
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} - Error: {ex.Message}");

                }
            

        }

    }
}
catch (Exception ex)
{
    log.SaveLogToFile(Path.Combine(properts.LogPath(),
                    $"log_ProdutosOUT {DateTime.Now:yyyy-MM-dd}.txt"),
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} - Error: {ex.Message}");

}