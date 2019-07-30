using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using TransparentWebViewXamForms;
using TransparentWebViewXF.Droid;
using Android.Content;

[assembly: ExportRenderer(typeof(TransparentWebView), typeof(TransparentWebViewRenderer))]
namespace TransparentWebViewXF.Droid
{
    public class TransparentWebViewRenderer : WebViewRenderer
    {
        public TransparentWebViewRenderer(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            // Setting the background as transparent
            this.Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }
    }
}
