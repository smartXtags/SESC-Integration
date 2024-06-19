using GrupoPatrimonial.Models;
using Models;
using SescIntegracaoLocal.Configs;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Xml.Linq;

ApplicationPropperts _prop = new ApplicationPropperts();
LogModel _log = new LogModel();
HttpClientHandler handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback =
    (HttpRequestMessage message, X509Certificate2 certificate,
        X509Chain chain, SslPolicyErrors sslErrors) => true;

HttpClient client = new HttpClient(handler);



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
        Descricao = "",
        Inativo = ""
    }
};

var jsonContent = JsonSerializer.Serialize(request);
var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

var httpRequest = new HttpRequestMessage
{
    Method = HttpMethod.Get,
    RequestUri = new Uri(_prop.ApiMxMGrupoPatrimonial()),
    Content = httpContent
};

Console.WriteLine("Solicitando dados do MxM");
var response = await client.SendAsync(httpRequest);
if (response.IsSuccessStatusCode)
{
    Console.WriteLine("Armazenando os dados recebidos do MxM");
    string responseCode = await response.Content.ReadAsStringAsync();

    var result = JsonSerializer.Deserialize<ResponseModel>(responseCode);

    if (result.Success)
    {
        List<GrupoModel> grupos = result.Data;

        Console.WriteLine("Formando o json");
        var xml = new XDocument(
            new XElement("msg",
                new XElement("command", "ImportGroup"),
                new XElement("terminal", "ERP"),
                new XElement("data",
                    grupos.Select(x =>  
                        new XElement("group",
                            new XElement("NAME", $"{x.Codigo} - {x.Descricao}")
                            )
                        )
                    )
                )
            );

        try
        {
            var responseXml = await client.PostAsync(_prop.ApiXtrack(),
                    new StringContent(xml.ToString(), System.Text.Encoding.UTF8, "application/xml"));

            var resultXml = await responseXml.Content.ReadAsStringAsync();

            if (resultXml == "<msg><status>0</status></msg>")
            {
                _log.SaveLogToFile(Path.Combine(_prop.LogPath(),
                                        $"logGrupo_{DateTime.Now:yyyy-MM-dd}.txt"),
                                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} - Dados enviados para o Xtrack, numero de grupos: {grupos.Count} ");
            }
            else
            {
                _log.SaveLogToFile(Path.Combine(_prop.LogPath(),
                                        $"logGrupo_{DateTime.Now:yyyy-MM-dd}.txt"),
                                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} - Erro ao enviar os grupos para o Xtrack");
            }
        }
        catch (Exception ex)
        {
            _log.SaveLogToFile(Path.Combine(_prop.LogPath(),
                                        $"logGrupo_{DateTime.Now:yyyy-MM-dd}.txt"),
                                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} - Status do consumo da API de grupos do MxM: {ex.Message}");
        }
    }
    else
    {
        Console.WriteLine("Erro ao armazenar os dados do MxM: " + result.Success.ToString());
        _log.SaveLogToFile(Path.Combine(_prop.LogPath(),
                            $"logGrupo_{DateTime.Now:yyyy-MM-dd}.txt"),
                            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} - Status do consumo da API de grupos do MxM: {result.Success.ToString()}");
    }
}
else
{
    Console.WriteLine("Erro ao consumir os dados do MxM: " + response.RequestMessage);
    _log.SaveLogToFile(Path.Combine(_prop.LogPath(),
                            $"logGrupo_{DateTime.Now:yyyy-MM-dd}.txt"),
                            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} - Error: {response.RequestMessage}");
}