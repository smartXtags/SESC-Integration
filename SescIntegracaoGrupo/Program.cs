using SescIntegracaoLocal.Configs;
using SescIntegracaoLocal.Models;
using SescIntegracaoProduto.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Xml.Linq;

ApplicationPropperts properts = new ApplicationPropperts();
HttpClient client = new HttpClient();
LogModel log = new LogModel();
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
        Codigo = "",
        TipoProduto = "",
        Descricao = "",
        GrupoSuperior = "",
        SinteticoAnalitico = "",
        Grau = "",
        GrupoPagamento = "",
        GrupoRecebimento = "",
        Exportado = "",
        Empresa = "",
        Filial = ""
    }
};

var jsonContent = JsonSerializer.Serialize(request);
var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

var httpRequest = new HttpRequestMessage
{
    Method = HttpMethod.Get,
    RequestUri = new Uri(properts.ApiMXM()),
    Content = httpContent
};


var response = await client.SendAsync(httpRequest);

if (response.IsSuccessStatusCode)
{
    string responseCode = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<ResponseModel>(responseCode);

    if (result.Success)
    {
        List<GrupoModel> grupoModels = result.Data;

        var secondApi = new XDocument(
            new XElement("msg",
                new XElement("command", "ImportGroup"),
                new XElement("terminal", "ERP"),
                new XElement("data",
                    grupoModels.Select(groupModel =>
                        new XElement("group",
                        new XElement("NAME", groupModel.Descricao)
                        )
                       )
                    )
                )
            );

        try
        {
            var responseXml = await client.PostAsync(properts.ApiXtrack(),
            new StringContent(secondApi.ToString(), System.Text.Encoding.UTF8, "application/xml"));

            string resultXml = await responseXml.Content.ReadAsStringAsync();

            if (resultXml == "<msg><status>0</status></msg>")
            {
                foreach (var model in grupoModels)
                {
                    log.SaveLogToFile(Path.Combine(properts.LogPath(),
                                $"log_{DateTime.Now:yyyy-MM-dd}.txt"),
                                $"Data registro: {DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} " +
                                $"Item implementado:" +
                                $" {model.Descricao}");
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