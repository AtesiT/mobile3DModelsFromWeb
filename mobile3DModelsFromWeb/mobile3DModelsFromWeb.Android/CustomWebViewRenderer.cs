using Android.Content;
using Android.Webkit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using mobile3DModelsFromWeb.Droid;

[assembly: ExportRenderer(typeof(Xamarin.Forms.WebView), typeof(CustomWebViewRenderer))]
namespace mobile3DModelsFromWeb.Droid
{
    public class CustomWebViewRenderer : WebViewRenderer
    {
        public CustomWebViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // Включаем JavaScript
                Control.Settings.JavaScriptEnabled = true;
                Control.Settings.DomStorageEnabled = true;
                Control.Settings.AllowFileAccess = true;
                Control.Settings.AllowContentAccess = true;
                Control.Settings.AllowFileAccessFromFileURLs = true;
                Control.Settings.AllowUniversalAccessFromFileURLs = true;

                // Включаем аппаратное ускорение
                Control.Settings.SetRenderPriority(WebSettings.RenderPriority.High);

                // Улучшаем производительность
                Control.Settings.MediaPlaybackRequiresUserGesture = false;

                // Включаем кэширование
                Control.Settings.CacheMode = CacheModes.Normal;
            }
        }
    }
}