using Android.Content;
using scichartaxis.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(scichartaxis.Native.MeasurementGraphView), typeof(MeasurementGraphViewRenderer))]

namespace scichartaxis.Droid.CustomRenderers
{
    public class MeasurementGraphViewRenderer : ViewRenderer<scichartaxis.Native.MeasurementGraphView, MeasurementGraphView>
    {
        private MeasurementGraphView _control;

        public MeasurementGraphViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<scichartaxis.Native.MeasurementGraphView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                if (e.OldElement.Data != null)
                {
                    e.OldElement.Data.CollectionChanged -= _control.OnDataCollectionChanged;
                }
                e.OldElement.PropertyChanged -= _control.OnCoreControlPropertyChanged;
            }

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    _control = new MeasurementGraphView(Context, e.NewElement);
                    SetNativeControl(_control);
                }

                if (e.NewElement.Data != null)
                {
                    e.NewElement.Data.CollectionChanged += _control.OnDataCollectionChanged;
                }
                e.NewElement.PropertyChanged += _control.OnCoreControlPropertyChanged;

                _control.ReinitializeData();
            }
        }
    }
}