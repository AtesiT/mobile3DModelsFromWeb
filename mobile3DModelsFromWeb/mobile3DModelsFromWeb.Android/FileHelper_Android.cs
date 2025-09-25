using System.IO;
using Xamarin.Forms;
using mobile3DModelsFromWeb.Droid; // Убедитесь, что пространство имен правильное

// Эта строка "регистрирует" наш класс в системе DependencyService.
// Теперь, когда мы вызовем DependencyService.Get<IFileHelper>() на Android,
// мы получим экземпляр именно этого класса.
[assembly: Dependency(typeof(FileHelper_Android))]
namespace mobile3DModelsFromWeb.Droid
{
    public class FileHelper_Android : IFileHelper
    {
        public string GetLocalFileHtml(string filename)
        {
            // На Android файлы из папки Assets доступны через Assets.Open()
            using (var stream = Android.App.Application.Context.Assets.Open(filename))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public string GetBaseUrl()
        {
            // Это специальный префикс для доступа к файлам в папке Assets из WebView
            return "file:///android_asset/";
        }
    }
}
