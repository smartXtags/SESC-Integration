using SescIntegracaoLocal.Configs;
using SescIntegracaoLocal.Models;
using SescIntegracaoProduto.Models;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Transactions;
using System.Xml.Linq;

HttpClientHandler handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback =
    (HttpRequestMessage message, X509Certificate2 certificate,
        X509Chain chain, SslPolicyErrors sslErrors) => true;
HttpClient client = new HttpClient(handler);
ApplicationPropperts config = new ApplicationPropperts();
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
        Descricao = "SESC"
    }
};

var jsonContent = JsonSerializer.Serialize(request);

var response = await client.PostAsync(config.ApiMXM(),
    new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json"));

if (response.IsSuccessStatusCode)
{
    string responseCode = await response.Content.ReadAsStringAsync();


    var result = JsonSerializer.Deserialize<ResponseModel>(responseCode);

    if (result.Success)
    {
        List<CostCenterModel> costCenterModels = result.Data;
            DateTime timestamp = DateTime.Now;

        foreach (var costCenterModel in costCenterModels)
        {
            timestamp = DateTime.Now;

            costCenterModel.DataModificacao = timestamp.ToString("dd/MM/yyyy hh:mm:ss");
        }
        var secondApiXml = new XDocument(
                    new XElement("msg",
                        new XElement("command", "ImportCostCenter"),
                        new XElement("terminal", "ERP"),
                        new XElement("data",
                            costCenterModels.Select(costCenters =>
                                new XElement("costcenter",
                                    new XElement("NAME", costCenters.Descricao),
                                    new XElement("LAST_MODIFIED", costCenters.DataModificacao)
                                )
                            )
                        )
                )
             );

        var responseXml = await client.PostAsync(config.ApiXtrack(),
            new StringContent(secondApiXml.ToString(), System.Text.Encoding.UTF8, "application/xml"));

        string resultXml = await responseXml.Content.ReadAsStringAsync();
        try
        {
            if (resultXml == "<msg><status>0</status></msg>")
            {
                foreach (var model in costCenterModels)
                {
                    log.SaveLogToFile(Path.Combine(config.LogPath(),
                                $"log_{DateTime.Now:yyyy-MM-dd}.txt"),
                                $"Data registro: {DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} " +
                                $"Item implementado:" +
                                $" {model.Descricao}");
                }
            }
            else
            {
                log.SaveLogToFile(Path.Combine(config.LogPath(),
                        $"log_{DateTime.Now:yyyy-MM-dd}.txt"),
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Error: {resultXml}");
            }
        }
        catch (Exception ex)
        {

            log.SaveLogToFile(Path.Combine(config.LogPath(),
                    $"log_{DateTime.Now:yyyy-MM-dd}.txt"),
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Error: {ex.Message}");
        }

    }
    else
    {
        log.SaveLogToFile(Path.Combine(config.LogPath(),
                            $"log_{DateTime.Now:yyyy-MM-dd}.txt"),
                            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Error: {result.Message}");
    }
}
else
{
    log.SaveLogToFile(Path.Combine(config.LogPath(),
                            $"log_{DateTime.Now:yyyy-MM-dd}.txt"),
                            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Error: {response.RequestMessage}");
}