using Xamarin.Forms;
using TransparentWebViewXamForms;
using Xamarin.Forms.Platform.iOS;
using TransparentWebViewXF.iOS;

[assembly: ExportRenderer(typeof(TransparentWebView), typeof(TransparentWebViewRenderer))]
namespace TransparentWebViewXF.iOS
{
	public class TransparentWebViewRenderer : WebViewRenderer
	{
		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			// Setting the background as transparent
			this.Opaque = false;
			this.BackgroundColor = Color.Transparent.ToUIColor();
		}
	}
}
