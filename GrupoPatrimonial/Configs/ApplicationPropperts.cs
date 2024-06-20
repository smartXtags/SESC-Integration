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
            var directoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(directoryPath, "Application.Propperties");

            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                string apiXtrack = lines.FirstOrDefault(line => line.StartsWith("ApiMxMGrupoPatrimonial="));
                string apiIsolada = "";

                if (apiXtrack != null || apiXtrack != "")
                {
                    int index = apiXtrack.IndexOf("=") + 1;

                    apiIsolada = apiXtrack.Substring(index);
                }
                return apiIsolada;
            }
            else
            {
                throw new FileNotFoundException($"O arquivo app.settings não foi encontrado em {directoryPath}");
            }
            
        }
        public string LogPath()
        {
            var directoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(directoryPath, "Application.Propperties");

            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                string apiXtrack = lines.FirstOrDefault(line => line.StartsWith("LogDirectory="));
                string apiIsolada = "";

                if (apiXtrack != null || apiXtrack != "")
                {
                    int index = apiXtrack.IndexOf("=") + 1;

                    apiIsolada = apiXtrack.Substring(index);
                }
                return apiIsolada;
            }
            else
            {
                throw new FileNotFoundException($"O arquivo app.settings não foi encontrado em {directoryPath}");
            }
        }
        public string ApiXtrack()
        {
            var directoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(directoryPath, "Application.Propperties");

            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                string apiXtrack = lines.FirstOrDefault(line => line.StartsWith("ApiXtrack="));
                string apiIsolada = "";

                if (apiXtrack != null || apiXtrack != "")
                {
                    int index = apiXtrack.IndexOf("=") + 1;

                    apiIsolada = apiXtrack.Substring(index);
                }
                return apiIsolada;
            }
            else
            {
                throw new FileNotFoundException($"O arquivo app.settings não foi encontrado em {directoryPath}");
            }
        }
    }
}
