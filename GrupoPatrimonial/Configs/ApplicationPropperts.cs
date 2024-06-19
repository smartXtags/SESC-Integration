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
        public string ApiMxMGrupoPatrimonial()
        {
            var lines = File.ReadAllLines("Application.Propperties");
            string apiMXM = lines.FirstOrDefault(line => line.StartsWith("ApiMxMGrupoPatrimonial="));
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
            var lines = File.ReadAllLines("Application.Propperties");
            string logPath = lines.FirstOrDefault(line => line.StartsWith("LogDirectory="));

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
            var lines = File.ReadAllLines("Application.Propperties");
            string apiXtack = lines.FirstOrDefault(line => line.StartsWith("ApiXtrack="));

            string logIsolado = "";
            if (apiXtack != null || apiXtack != "")
            {
                int index = apiXtack.IndexOf("=") + 1;
                logIsolado += apiXtack.Substring(index);
            }
            return logIsolado;
        }
    }
}
