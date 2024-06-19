
namespace Models
{
    internal class LogModel
    {
        public void SaveLogToFile(string filePath, string logEntry)
        {
            File.AppendAllText(filePath, logEntry + Environment.NewLine);
        }
    }
}
