using SescIntegracaoLocal.Configs;
using SescIntegracaoLocal.Models;
using SescIntegracaoProduto.Models;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Xml.Linq;

ApplicationPropperts properts  = new ApplicationPropperts();
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
    data = new
    {
        Codigo = ""
    }
};

var request2 = new
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

var request3 = new
{
    AutheticationToken = new
    {
        Username = "INTEGRACAOPE",
        Password = "Fpjuu4fj_-04e1vj",
        EnvironmentName = "SESCPEHOM"
    },
    Data = new
    {
        Empresa = "",
        Filial = ""
    }
};


var jsonContent = JsonSerializer.Serialize(request);

var jsonContent2 = JsonSerializer.Serialize(request2);

var jsonContent3 = JsonSerializer.Serialize(request3);

var response = await client.PostAsync(properts.ApiMXM(),
    new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json"));

var response2 = await client.PostAsync(properts.ApiMXMProduto(),
    new StringContent(jsonContent2, System.Text.Encoding.UTF8, "application/json"));

var response3 = await client.PostAsync(properts.ApiMXMFilial(),
    new StringContent(jsonContent3, System.Text.Encoding.UTF8, "application/json"));   

if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode && response3.IsSuccessStatusCode)
{
    string responseCode = await response.Content.ReadAsStringAsync();

    string responseCode2 = await response2.Content.ReadAsStringAsync();

    string responseCode3 = await response3.Content.ReadAsStringAsync();

    var result = JsonSerializer.Deserialize<ResponseModel>(responseCode);

    var result2 = JsonSerializer.Deserialize<ResponseModel2>(responseCode2);

    var result3 = JsonSerializer.Deserialize<ResponseModel3>(responseCode3);

    if (result.Success && result2.Success && result3.Success)
    {
        List<LocalModel> localModels = result.Data.ToList();

        List<ProdutoModel> produtoModels = result2.Data.ToList();

        List<FilialModel> filialModels = result3.Data.ToList();

        foreach (var localModel in localModels)
        {
            var filialCorrespondente = produtoModels.FirstOrDefault(x => x.Local == localModel.Codigo);
            if (filialCorrespondente == null)
            {
                localModel.Filial = "";
            }
            else
            {
                localModel.Filial = filialCorrespondente.Filial;
            }

            var filialNomeCorrespondente = filialModels.FirstOrDefault(x => x.CodigoFilial == localModel.Filial);

            if (filialNomeCorrespondente == null || localModel.Filial == "")
            {
                localModel.Filial = "Sem Filial";
                localModel.Cidade = "Sem Cidade";
                localModel.UF = "Sem UF";
            }
            else
            {
                localModel.Filial = filialNomeCorrespondente.NomeFilial;
                localModel.UF = filialNomeCorrespondente.UF;
                localModel.Cidade = filialNomeCorrespondente.Cidade;
            }
        }

        var secondiApiXml = new XDocument(
            new XElement("msg",
                new XElement("command", "ImportLocation"),
                new XElement("terminal", "ERP"),
                new XElement("data",
                    localModels.Select(localModels =>
                    new XElement("location",
                        new XElement("NAME", $"{localModels.UF}/{localModels.Cidade}/{localModels.Filial}/{localModels.Codigo} - {localModels.Descricao}"),
                        new XElement("ALLOCABLE", "1"),
                        new XElement("USRDATA1", localModels.CodigoResponsavel),
                        new XElement("USRDATA2", localModels.NomeResponsavel),
                        new XElement("USRDATA3", localModels.Codigo),
                        new XElement("USRDATA4", ""),
                        new XElement("USRDATA5", ""),
                        new XElement("USRDATA6", ""),
                        new XElement("USRDATA7", ""),
                        new XElement("USRDATA8", ""),
                        new XElement("USRDATA9", ""),
                        new XElement("IDETYPE1", "BARCODE"),
                        new XElement("IDECODE1", ""),
                        new XElement("IDETYPE2", ""),
                        new XElement("IDECODE2", ""),
                        new XElement("IDETYPE3", ""),
                        new XElement("IDECODE3", ""),
                        new XElement("IDETYPE4", ""),
                        new XElement("IDECODE4", "")
                            )
                        )
                    )
                )
            );

        try
        {
            var responseXml = await client.PostAsync(properts.ApiXtrack(),
            new StringContent(secondiApiXml.ToString(), System.Text.Encoding.UTF8,
            "application/xml"));

            var resultXml = await responseXml.Content.ReadAsStringAsync();

            if (resultXml == "<msg><status>0</status></msg>")
            {
                foreach (var model in localModels)
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
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Error: {response.RequestMessage}");
        }
        
    }
    else
    {
        log.SaveLogToFile(Path.Combine(properts.LogPath(),
                    $"log_{DateTime.Now:yyyy-MM-dd}.txt"),
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Error: {result.Message}");
    }

}
else
{
    log.SaveLogToFile(Path.Combine(properts.LogPath(),
                    $"log_{DateTime.Now:yyyy-MM-dd}.txt"),
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Error: {response.RequestMessage}");
}