using Android.Webkit;
using System.IO;

namespace mobile3DModelsFromWeb.Droid
{
    public class HybridWebViewClient : WebViewClient
    {
        // Этот метод вызывается каждый раз, когда WebView хочет загрузить какой-либо ресурс (html, js, gltf, etc.)
        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            // Мы будем перехватывать только запросы к нашему выдуманному домену "appassets.local"
            if (request.Url.Host == "appassets.local")
            {
                // Получаем путь к файлу из URL (например, /model/scene.gltf)
                // Substring(1) убирает первый слэш '/'
                var filePath = request.Url.Path.Substring(1);

                try
                {
                    // Открываем файл из папки Assets
                    var stream = view.Context.Assets.Open(filePath);

                    // Определяем MIME-тип файла по его расширению
                    var mimeType = GetMimeType(filePath);

                    // Создаем и возвращаем ответ, который "увидит" WebView
                    // Он будет думать, что получил успешный ответ от сервера
                    return new WebResourceResponse(mimeType, "UTF-8", stream);
                }
                catch (IOException)
                {
                    // Если файл не найден, возвращаем null, что приведет к ошибке 404 в WebView
                    return null;
                }
            }

            // Для всех остальных запросов (например, к rutube.ru) используем стандартное поведение
            return base.ShouldInterceptRequest(view, request);
        }

        // Вспомогательная функция для определения MIME-типа
        private string GetMimeType(string url)
        {
            string extension = Path.GetExtension(url).ToLower();
            switch (extension)
            {
                case ".js":
                    return "text/javascript";
                case ".html":
                    return "text/html";
                case ".css":
                    return "text/css";
                case ".gltf":
                    return "model/gltf+json";
                case ".glb":
                    return "model/gltf-binary";
                case ".bin":
                    return "application/octet-stream";
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
