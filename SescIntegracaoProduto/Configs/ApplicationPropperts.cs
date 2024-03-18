using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SescIntegracaoLocal.Configs
{
    internal class ApplicationPropperts
    {
        public string ApiMXM()
        {
            var lines = File.ReadAllLines("appsettings.txt");
            string apiMXM = lines.FirstOrDefault(line => line.StartsWith("Api="));
            string apiIsolada = "";

            if (apiMXM != null || apiMXM != "")
            {
                int index = apiMXM.IndexOf("=") + 1;

                apiIsolada = apiMXM.Substring(index);
            }
            return apiIsolada;
        }
        public string LogPath()
        {
            var lines = File.ReadAllLines("appsettings.txt");
            string logPath = lines.FirstOrDefault(line => line.StartsWith("logDirectory="));

            string logIsolado = "";
            if (logPath != null || logPath != "")
            {
                int index = logPath.IndexOf("=") + 1;
                logIsolado += logPath.Substring(index);
            }
            return logIsolado;
        }
        public string ApiXtrack()
        {
            var lines = File.ReadAllLines("appsettings.txt");
            string apiXtrack = lines.FirstOrDefault(line => line.StartsWith("apiXtrack="));
            string apiIsolada = "";
            if (apiXtrack != "" || apiXtrack != null)
            {
                int index = apiXtrack.IndexOf("=") + 1;
                apiIsolada = apiXtrack.Substring(index);
            }
            return apiIsolada;
        }
    }
}
