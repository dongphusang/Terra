package crc64296032e6dd374240;


public class SKGLViewRenderer
	extends crc64296032e6dd374240.SKGLViewRendererBase_2
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("SkiaSharp.Views.Maui.Controls.Compatibility.SKGLViewRenderer, SkiaSharp.Views.Maui.Controls.Compatibility", SKGLViewRenderer.class, __md_methods);
	}


	public SKGLViewRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == SKGLViewRenderer.class) {
			mono.android.TypeManager.Activate ("SkiaSharp.Views.Maui.Controls.Compatibility.SKGLViewRenderer, SkiaSharp.Views.Maui.Controls.Compatibility", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
		}
	}


	public SKGLViewRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == SKGLViewRenderer.class) {
			mono.android.TypeManager.Activate ("SkiaSharp.Views.Maui.Controls.Compatibility.SKGLViewRenderer, SkiaSharp.Views.Maui.Controls.Compatibility", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
		}
	}


	public SKGLViewRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == SKGLViewRenderer.class) {
			mono.android.TypeManager.Activate ("SkiaSharp.Views.Maui.Controls.Compatibility.SKGLViewRenderer, SkiaSharp.Views.Maui.Controls.Compatibility", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, System.Private.CoreLib", this, new java.lang.Object[] { p0, p1, p2 });
		}
	}


	public SKGLViewRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2, int p3)
	{
		super (p0, p1, p2, p3);
		if (getClass () == SKGLViewRenderer.class) {
			mono.android.TypeManager.Activate ("SkiaSharp.Views.Maui.Controls.Compatibility.SKGLViewRenderer, SkiaSharp.Views.Maui.Controls.Compatibility", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, System.Private.CoreLib:System.Int32, System.Private.CoreLib", this, new java.lang.Object[] { p0, p1, p2, p3 });
		}
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
