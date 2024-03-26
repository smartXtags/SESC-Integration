using SescIntegracaoLocal.Configs;
using SescIntegracaoLocal.Models;
using SescIntegracaoProduto.Models;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;

ApplicationPropperts properts = new ApplicationPropperts();
HttpClient client = new HttpClient();
LogModel log = new LogModel();

client.Timeout = TimeSpan.FromMinutes(100);

var request = new
{
    AutheticationToken = new
    {
        Username = "INTEGRACAOPE",
        Password = "Fpjuu4fj_-04e1vj",
        EnvironmentName = "SESCPEHOM"
    },
    Data = new
    {
        ConsultaItemPatrimonial = new
        {
            Empresa = "PE",
            Filial = "",
            GrupoPatrimonialDe = "",
            GrupoPatrimonialAte = "",
            CentrodeCustoDe = "",
            CentrodeCustoAte = "",
            PeriodoDe = "",
            PeriodoAte = "",
            Moeda = "",
            DataAlteracaoDe = "",
            DataAlteracaoAte = "",
            DataPesquisaDe = "",
            DataPesquisaAte = ""
        }
    }
};

var jsonContent = JsonSerializer.Serialize(request);


var response = await client.PostAsync(properts.ApiMXM(),
    new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json"));


if (response.IsSuccessStatusCode)
{
    string responseCode = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<ResponseModel>(responseCode);

    if (result.Success)
    {
        List<ProdutoModel> produtos = result.Data.
            GroupBy(p => p.Codigo)
            .Select(g => g.OrderByDescending(p => 
            string.IsNullOrWhiteSpace(p.DataHoraAlteracaoHistorico)
            ? DateTime.MinValue
            : DateTime.ParseExact(p.DataHoraAlteracaoHistorico, "dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR"))).First()).ToList();

        
        int batchSize = 3000;
        for (int i = 0; i <= produtos.Count; i += batchSize)
        {
            int count = Math.Min(batchSize, produtos.Count - i);
            var batch = produtos.GetRange(i, count);

            foreach (var item in batch)
            {
                if (item.Datadabaixa != "")
                {
                    item.Status = "10";


                }
                else
                {
                    item.Status = "11";
                }
            }

            var xml = new XDocument(
            new XElement("msg",
                new XElement("command", "ImportObject"),
                new XElement("terminal", "ERP"),
                new XElement("data",
                batch.Select(objeto =>
                    new XElement("object",
                        new XElement("ACTIVE", objeto.Status),
                        new XElement("IDCODE", objeto.Codigo),
                        new XElement("DESCRIPTION", objeto.Descricao),
                        new XElement("SERIALNUMBER", ""),
                        new XElement("QUANTITY", ""),
                        new XElement("ITEMMODEL_IDCODE", ""),
                        new XElement("DEPARTMENT_NAME", ""),
                        new XElement("CONDITION_NAME", ""),
                        new XElement("DISPOSITION_NAME", ""),
                        new XElement("LOCATION_NAME", objeto.Local),
                        new XElement("HOMELOCATION_NAME", ""),
                        new XElement("GROUP_NAME", $"{objeto.GrupoPatrimonial}"),
                        new XElement("CUSTODIAN_NAME", ""),
                        new XElement("DISPOSAL_NAME", ""),
                        new XElement("COSTCENTER_NAME", ""),
                        new XElement("CONTAINER_IDCODE", ""),
                        new XElement("LATITUDE", ""),
                        new XElement("LONGITUDE", ""),
                        new XElement("USRDATA1", objeto.DatadeAquisição),
                        new XElement("USRDATA2", ""),
                        new XElement("USRDATA3", ""),
                        new XElement("USRDATA4", ""),
                        new XElement("USRDATA5", ""),
                        new XElement("USRDATA6", ""),
                        new XElement("USRDATA7", ""),
                        new XElement("USRDATA8", ""),
                        new XElement("USRDATA9", ""),
                        new XElement("IDETYPE1", "BARCODE"),
                        new XElement("IDECODE1", objeto.Codigo),
                        new XElement("IDETYPE2", "RFID"),
                        new XElement("IDECODE2", objeto.Codigo),
                        new XElement("IDETYPE3", ""),
                        new XElement("IDECODE3", ""),
                        new XElement("IDETYPE4", ""),
                        new XElement("IDECODE4", ""),
                        new XElement("IMAGEFILE", "")
                                )
                            )
                        )
                    )
            );

            try
            {
                var responseXml = await client.PostAsync(properts.ApiXtrack(),
                new StringContent(xml.ToString()));

                string resultXml = await responseXml.Content.ReadAsStringAsync();

                if (resultXml == "<msg><status>0</status></msg>")
                {
                    foreach (var model in batch)
                    {
                        log.SaveLogToFile(Path.Combine(properts.LogPath(),
                            $"log_{DateTime.Now:yyyy-MM-dd}.txt"),
                            $"Data registro: {DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} " +
                            $"Item implementado:" +
                            $" {model.Codigo} " + $" {model.Descricao}");
                    }
                }
                else
                {
                    log.SaveLogToFile(Path.Combine(properts.LogPath(),
                        $"log_{DateTime.Now:yyyy-MM-dd}.txt"),
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Error: {response.RequestMessage}");
                }
            }
            catch (Exception ex)
            {
                log.SaveLogToFile(Path.Combine(properts.LogPath(),
                    $"log_{DateTime.Now:yyyy-MM-dd}.txt"),
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Error: {ex.Message}");

            }
            

            
        }
    }

}