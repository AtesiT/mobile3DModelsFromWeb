using System.IO;
using Foundation;
using Xamarin.Forms;
using mobile3DModelsFromWeb.iOS; // Убедитесь, что пространство имен правильное

[assembly: Dependency(typeof(FileHelper_iOS))]
namespace mobile3DModelsFromWeb.iOS
{
    public class FileHelper_iOS : IFileHelper
    {
        public string GetLocalFileHtml(string filename)
        {
            // На iOS файлы из папки Resources доступны через NSBundle
            string path = Path.Combine(NSBundle.MainBundle.BundlePath, filename);
            return File.ReadAllText(path);
        }

        public string GetBaseUrl()
        {
            // На iOS базовый URL - это просто путь к главной папке приложения
            return NSBundle.MainBundle.BundlePath;
        }
    }
}
