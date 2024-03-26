using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SescIntegracaoProduto.Models
{
    public class LogModel
    {
        public void SaveLogToFile(string filePath, string logEntry)
        {
            File.AppendAllText(filePath, logEntry + Environment.NewLine);
        }
    }
}
