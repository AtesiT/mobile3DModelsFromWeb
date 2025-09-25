using Xamarin.Forms;

// Пространство имен должно совпадать с названием вашего проекта
namespace mobile3DModelsFromWeb
{
    // 'partial' означает, что это часть класса, другая часть которого определена в XAML
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            // Эта функция связывает XAML и C#. Без нее MyWebView будет null.
            InitializeComponent();

            // Старый код мы отсюда уберем и заменим его вызовом DependencyService
            LoadWebViewContent();
        }

        void LoadWebViewContent()
        {
            // 1. Получаем реализацию нашего сервиса для текущей платформы (Android/iOS)
            IFileHelper fileHelper = DependencyService.Get<IFileHelper>();
            if (fileHelper == null) return;

            // 2. Загружаем HTML из файла с помощью нашего сервиса
            string htmlContent = fileHelper.GetLocalFileHtml("index.html");

            // 3. Получаем базовый URL, чтобы WebView знал, где искать файлы типа './model/scene.gltf'
            string baseUrl = fileHelper.GetBaseUrl();

            var htmlSource = new HtmlWebViewSource
            {
                Html = htmlContent,
                BaseUrl = baseUrl
            };

            MyWebView.Source = htmlSource;
        }
    }
}
