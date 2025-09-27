using Android.Content;
using Android.Webkit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using mobile3DModelsFromWeb;
using mobile3DModelsFromWeb.Droid;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace mobile3DModelsFromWeb.Droid
{
    public class HybridWebViewRenderer : WebViewRenderer
    {
        public HybridWebViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // Enable JavaScript
                Control.Settings.JavaScriptEnabled = true;
                // Enable DOM Storage
                Control.Settings.DomStorageEnabled = true;
                // Enable hardware acceleration
                Control.SetLayerType(Android.Views.LayerType.Hardware, null);
                // Allow file access
                Control.Settings.AllowFileAccess = true;
                Control.Settings.AllowFileAccessFromFileURLs = true;
                Control.Settings.AllowUniversalAccessFromFileURLs = true;

                // Set custom WebViewClient
                Control.SetWebViewClient(new HybridWebViewClient());

                // Enable WebView debugging
                Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
            }
        }
    }
}