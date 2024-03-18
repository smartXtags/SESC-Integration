using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SescIntegracaoProduto.Models
{
    internal class LogModel
    {
        public void SaveLogToFile(string filePath, string logEntry)
        {
            File.AppendAllText(filePath, logEntry + Environment.NewLine);
        }
    }
}
