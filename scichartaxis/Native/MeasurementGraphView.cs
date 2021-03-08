using System.Collections.ObjectModel;
using scichartaxis.Data;
using Xamarin.Forms;

namespace scichartaxis.Native
{
    public class MeasurementGraphView : View
    {
        public ObservableCollection<MeasurementPoint> Data
        {
            get => (ObservableCollection<MeasurementPoint>)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public bool IsOxygenVisible
        {
            get { return (bool)GetValue(IsOxygenVisibleProperty); }
            set { SetValue(IsOxygenVisibleProperty, value); }
        }
        public bool IsTemperatureVisible
        {
            get { return (bool)GetValue(IsTemperatureVisibleProperty); }
            set { SetValue(IsTemperatureVisibleProperty, value); }
        }
        public bool IsPressureVisible
        {
            get { return (bool)GetValue(IsPressureVisibleProperty); }
            set { SetValue(IsPressureVisibleProperty, value); }
        }
        public bool IsZoomable
        {
            get { return (bool)GetValue(IsZoomableProperty); }
            set { SetValue(IsZoomableProperty, value); }
        }
        public Thickness ChartSurfaceMargin
        {
            get { return (Thickness)GetValue(ChartSurfaceMarginProperty); }
            set { SetValue(ChartSurfaceMarginProperty, value); }
        }

        public static readonly BindableProperty DataProperty = BindableProperty.Create("Data", typeof(ObservableCollection<MeasurementPoint>), typeof(MeasurementGraphView), default(ObservableCollection<MeasurementPoint>));
        public static readonly BindableProperty IsOxygenVisibleProperty = BindableProperty.Create("IsOxygenVisible", typeof(bool), typeof(MeasurementGraphView), true);
        public static readonly BindableProperty IsTemperatureVisibleProperty = BindableProperty.Create("IsTemperatureVisible", typeof(bool), typeof(MeasurementGraphView), default(bool));
        public static readonly BindableProperty IsPressureVisibleProperty = BindableProperty.Create("IsPressureVisible", typeof(bool), typeof(MeasurementGraphView), default(bool));
        public static readonly BindableProperty IsZoomableProperty = BindableProperty.Create("IsZoomable", typeof(bool), typeof(MeasurementGraphView), default(bool));
        public static readonly BindableProperty ChartSurfaceMarginProperty = BindableProperty.Create("ChartSurfaceMargin", typeof(Thickness), typeof(MeasurementGraphView), default(Thickness));
    }
}
