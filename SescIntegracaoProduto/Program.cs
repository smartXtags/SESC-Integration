using SescIntegracaoLocal.Configs;
using SescIntegracaoLocal.Models;
using SescIntegracaoProduto.Models;
using System.Globalization;
using System.Net.Security;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Xml.Linq;

ApplicationPropperts properts = new ApplicationPropperts();


HttpClientHandler handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback =
    (HttpRequestMessage message, X509Certificate2 certificate,
        X509Chain chain, SslPolicyErrors sslErrors) => true;

HttpClient client = new HttpClient(handler);
LogModel log = new LogModel();

client.Timeout = TimeSpan.FromMinutes(1000);

// ZONA RESPONSÁVEL POR FORMAR OS JSON E CONSUMIR AS INFORMAÇÕES DE APIS MENOS ROBUSTAS\\

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
            Empresa = "",
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

var request2 = new
{
    command = "GetLocation",
    terminal = "ERP",
};

var request4 = new
{
    command = "GetDepartment",
    terminal = "ERP",
};

var request5 = new
{
    command = "GetGroup",
    terminal = "ERP",
};
var request6 = new
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

var jsonContent4 = JsonSerializer.Serialize(request4);

var jsonContent5 = JsonSerializer.Serialize(request5);

var jsonContent6 = JsonSerializer.Serialize(request6);

Console.WriteLine("Consumindo api de Produtos");
var response = await client.PostAsync(properts.ApiMXM(),
    new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json"));

Console.WriteLine("Consumindo API de locais Xtrack");
var response2 = await client.PostAsync(properts.ApiXtrack(),
    new StringContent(jsonContent2, System.Text.Encoding.UTF8, "application/json"));

Console.WriteLine("Consumindo API de departamentos Xtrack");
var response4 = await client.PostAsync(properts.ApiXtrack(),
    new StringContent(jsonContent4, System.Text.Encoding.UTF8, "application/json"));

Console.WriteLine("Consumindo API de grupos do Xtrack");
var response5 = await client.PostAsync(properts.ApiXtrack(),
    new StringContent(jsonContent5, System.Text.Encoding.UTF8, "application/json"));

Console.WriteLine("Consumindo departamentos do MxM");
var response6 = await client.PostAsync(properts.ApiMxMFiliais(),
    new StringContent(jsonContent6, System.Text.Encoding.UTF8, "application/json"));

// ========================= FIM DA ZONA =========================\\

if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode && response4.IsSuccessStatusCode && response5.IsSuccessStatusCode && response6.IsSuccessStatusCode)
{
    // CAPTANDO AS INFORMAÇÕES E ARMAZENANDO EM VARIÁVEIS \\
    string responseCode = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<ResponseModel>(responseCode);

    string responseCode2 = await response2.Content.ReadAsStringAsync();
    var result2 = JsonSerializer.Deserialize<ResponseModel2>(responseCode2);

    string responseCode4 = await response4.Content.ReadAsStringAsync();
    var result4 = JsonSerializer.Deserialize<ResponseModel4>(responseCode4);

    string responseCode5 = await response5.Content.ReadAsStringAsync();
    var result5 = JsonSerializer.Deserialize<ResponseModel5>(responseCode5);

    string responseCode6 = await response6.Content.ReadAsStringAsync();
    var result6 = JsonSerializer.Deserialize<ResponseModel6>(responseCode6);

    //========================= FIM DA ZONA =========================\\
    if (result.Success && result2 != null && result4 != null && result5 != null && result6.Success)
    {
        List<ProdutoModel> produtos = result.Data
            .GroupBy(p => p.Codigo)
            .Select(g => g.OrderByDescending(p => 
            string.IsNullOrWhiteSpace(p.DataHoraAlteracaoHistorico)
            ? DateTime.MinValue
            : DateTime.ParseExact(p.DataHoraAlteracaoHistorico, "dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR"))).First()).ToList();

        List<LocalModel> localModels = result2.data;
        
        List<LocalModel> localModels2 = result2.data;

        //======================= ZONA DO JOB =======================\\
        //Job responsável por consumir os itens patrimoniais por filial

        List<ConsultaItemPatrimonialModel> itemPatrimoniais = new List<ConsultaItemPatrimonialModel>();

        foreach (var filial in result6.Data)
        {
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
                    ConsultaItemPatrimonial = new
                    {
                        CodigoEmpresa = "PE",
                        CodigoFilial = filial.CodigoFilial,
                        CodigoItemDe = "",
                        CodigoAnexoDe = "",
                        CodigoItemAte = "",
                        CodigoAnexoAte = "",
                        Fornecedor = "",
                        Documento = "",
                        DataDeAquisicaoDe = "",
                        DataDeAquisicaoAte = "",
                        GrupoPatrimonial = "",
                        SubGrupoPatrimonial = "",
                        CentroDeCusto = "",
                        Projeto = "",
                        Local = "",
                        Responsavel = "",
                        MotivoDaBaixa = "",
                        DataDaBaixa = "",
                        NumeroDeSerie = ""
                    }
                }
            };

            var jsonContent3 = JsonSerializer.Serialize(request3);

            Console.WriteLine("Consumindo API de Item Patrimonial referente a filial: " + filial.CodigoFilial);
            var response3 = await client.PostAsync(properts.ApiMXMConsultaItemPatrimonial(),
                new StringContent(jsonContent3, System.Text.Encoding.UTF8, "application/json"));

            if (response3.IsSuccessStatusCode)
            {
                string responseCode3 = await response3.Content.ReadAsStringAsync();
                var result3 = JsonSerializer.Deserialize<ResponseModel3>(responseCode3);

                if (result3.Success)
                {
                    foreach (var item in result3.Data)
                    {
                        itemPatrimoniais.Add(item);
                    }
                }
            }
        }

        //======================= ZONA DO JOB =======================\\


        List<DepartmentModel> departaments = result4.data;
        
        List<GroupModel> groups = result5.data;

        int batchSize = 3000;
        for (int i = 0; i <= produtos.Count; i += batchSize)
        { 
            int count = Math.Min(batchSize, produtos.Count - i);
            var batch = produtos.GetRange(i, count);
            foreach (var item in batch)
            {
                
                var descricaoCorrepondente = localModels.FirstOrDefault(x => x.USR3 == item.Local);

                var itemPatrimonialCorrepondente = itemPatrimoniais.FirstOrDefault(x => x.CodigoItem == item.Codigo);

                var grupoCorrespondente = groups.FirstOrDefault(x => x.NAME.Contains(item.GrupoPatrimonial));

                if (grupoCorrespondente != null)
                {
                    item.GrupoPatrimonial = $"{grupoCorrespondente.NAME}";
                }

                if (itemPatrimonialCorrepondente != null)
                {
                    item.DescricaoComplementar = itemPatrimonialCorrepondente.DescricaoComplementar;
                    item.ValorDeAquisicao = itemPatrimonialCorrepondente.ValorDeAquisicao;
                    item.ValorDepreciado = itemPatrimonialCorrepondente.ValorDepreciado;
                    item.ValorResidual = itemPatrimonialCorrepondente.ValorResidual;
                }
                else
                {
                    item.DescricaoComplementar = "";
                    item.ValorDeAquisicao = "";
                    item.ValorDepreciado = "";
                    item.ValorResidual = "";
                }

                if (descricaoCorrepondente != null)
                {
                    item.Local = descricaoCorrepondente.CODE;
                }
                else
                {
                    item.Local = "";
                }

                string[] partesLocal = item.Local.Split('/');

                if (partesLocal.Length >= 4)
                {
                    string filialItem = partesLocal[2];
                    var departamento = departaments.FirstOrDefault(x => x.NAME == filialItem);

                    if (departamento is null)
                    {
                        item.Departamento = "";
                    }
                    else
                    {
                        item.Departamento = departamento.NAME;
                    }

                }

                if (item.Datadabaixa != "")
                {
                    item.Status = "10";
                    item.Disposicao = "INDISPONIVEL";
                }
                else
                {
                    item.Status = "11";
                    item.Disposicao = "DISPONIVEL";
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
                        new XElement("DEPARTMENT_NAME", objeto.Departamento),
                        new XElement("CONDITION_NAME", ""),
                        new XElement("DISPOSITION_NAME", objeto.Disposicao),
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
                        new XElement("USRDATA5", objeto.DescricaoComplementar),
                        new XElement("USRDATA6", objeto.ValorDeAquisicao),
                        new XElement("USRDATA7", objeto.ValorDepreciado),
                        new XElement("USRDATA8", objeto.ValorResidual),
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
                Console.WriteLine("Enviando informações para o Xtrack");
                var responseXml = await client.PostAsync(properts.ApiXtrack(),
                new StringContent(xml.ToString(), System.Text.Encoding.UTF8, "application/xml"));

                string resultXml = await responseXml.Content.ReadAsStringAsync();

                GC.Collect();

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

                batch.Clear();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

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