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

var jsonContent = JsonSerializer.Serialize(request);

var response = await client.PostAsync(properts.ApiMXM(),
    new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json"));

if (response.IsSuccessStatusCode)
{
    string responseCode = await response.Content.ReadAsStringAsync();

    var result = JsonSerializer.Deserialize<ResponseModel>(responseCode);
    if (result.Success)
    {
        List<LocalModel> localModels = result.Data.ToList();

        var secondiApiXml = new XDocument(
            new XElement("msg",
                new XElement("command", "ImportLocation"),
                new XElement("terminal", "ERP"),
                new XElement("data",
                    localModels.Select(localModels =>
                    new XElement("location",
                        new XElement("NAME", @"SESCPE/" + localModels.Descricao),
                        new XElement("ALLOCABLE", "1"),
                        new XElement("USRDATA1", localModels.CodigoResponsavel),
                        new XElement("USRDATA2", localModels.NomeResponsavel),
                        new XElement("USRDATA3", ""),
                        new XElement("USRDATA4", ""),
                        new XElement("USRDATA5", ""),
                        new XElement("USRDATA6", ""),
                        new XElement("USRDATA7", ""),
                        new XElement("USRDATA8", ""),
                        new XElement("USRDATA9", ""),
                        new XElement("IDETYPE1", ""),
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