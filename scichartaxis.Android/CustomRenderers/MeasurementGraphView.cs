using Android.Content;
using Android.Widget;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Data.Model;
using SciChart.Drawing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Views;
using SciChart.Charting.Model;
using SciChart.Charting.Modifiers;
using Xamarin.Forms;
using SciChart.Charting.Visuals.Legend;
using SciChart.Charting.Themes;
using SciChart.Charting;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;
using scichartaxis.Data;

namespace scichartaxis.Droid.CustomRenderers
{
    public class MeasurementGraphView : FrameLayout, INativeElementView
    {
        #region Constants
        private const string X_AXIS_ID = "xAxis";
        private const string OXYGEN_AXIS_ID = "oxygenAxis";
        private const string TEMPERATURE_AXIS_ID = "temperatureAxis";
        private const string PRESSURE_AXIS_ID = "pressureAxis";
        #endregion

        #region Fields
        private FastLineRenderableSeries _oxygenSeries;
        private FastLineRenderableSeries _temperatureSeries;
        private FastLineRenderableSeries _pressureSeries;

        private XyDataSeries<DateTime, double> _oxygenData;
        private XyDataSeries<DateTime, double> _temperatureData;
        private XyDataSeries<DateTime, double> _pressureData;
        #endregion

        #region Properties
        public SciChartSurface ChartSurface { get; set; }
        public Native.MeasurementGraphView NativeControl { get; private set; }
        public Element Element => NativeControl;
        #endregion

        #region Constructor
        public MeasurementGraphView(Context context, Native.MeasurementGraphView nativeControl) : base(context)
        {
            NativeControl = nativeControl;

            // create view
            InitializeGraph(context);
        }
        #endregion

        #region Private Methods
        private void InitializeGraph(Context context)
        {
            // init shared resources
            var oxygenColor = Android.Graphics.Color.Blue;
            var temperatureColor = Android.Graphics.Color.Red;
            var pressureColor = Android.Graphics.Color.Teal;

            var thickness = 1.5f * Resources.DisplayMetrics.Density;
            var fontSize = 14f * Resources.DisplayMetrics.Density;

            // init surface
            ChartSurface = new SciChartSurface(context)
            {
                RenderableSeriesAreaBorderStyle = new SolidPenStyle(Color.Black, 0)
            };

            // init axes
            var dateAxis = new DateAxis(context)
            {
                AutoRange = NativeControl.IsZoomable ? AutoRange.Once : AutoRange.Always,
                GrowBy = new DoubleRange(0.05, 0.05),
                DrawMinorGridLines = false,
                TickLabelStyle = new FontStyle(fontSize, Color.Black),
                TitleStyle = new FontStyle(fontSize, Color.Black),
                AxisId = X_AXIS_ID
            };
            var oxygenAxis = new NumericAxis(context)
            {
                AutoRange = NativeControl.IsZoomable ? AutoRange.Once : AutoRange.Always,
                AxisAlignment = AxisAlignment.Left,
                GrowBy = new DoubleRange(0.05, 0.05),
                AxisId = OXYGEN_AXIS_ID,
                AxisTitle = "Oxygen",
                Visibility = ConvertBoolToInt(NativeControl.IsOxygenVisible),
                TickLabelStyle = new FontStyle(fontSize, oxygenColor),
                TitleStyle = new FontStyle(fontSize, oxygenColor),
                DrawMinorGridLines = false,
                IsPrimaryAxis = true,
            };
            var temperatureAxis = new NumericAxis(context)
            {
                AutoRange = NativeControl.IsZoomable ? AutoRange.Once : AutoRange.Always,
                AxisAlignment = AxisAlignment.Right,
                GrowBy = new DoubleRange(0.05, 0.05),
                AxisId = TEMPERATURE_AXIS_ID,
                AxisTitle = "Temperature",
                Visibility = ConvertBoolToInt(NativeControl.IsTemperatureVisible),
                TickLabelStyle = new FontStyle(fontSize, temperatureColor),
                TitleStyle = new FontStyle(fontSize, temperatureColor),
                DrawMinorGridLines = false,
                IsPrimaryAxis = false
            };
            var pressureAxis = new NumericAxis(context)
            {
                AutoRange = NativeControl.IsZoomable ? AutoRange.Once : AutoRange.Always,
                AxisAlignment = AxisAlignment.Right,
                GrowBy = new DoubleRange(0.05, 0.05),
                AxisId = PRESSURE_AXIS_ID,
                AxisTitle = "Pressure",
                Visibility = ConvertBoolToInt(NativeControl.IsPressureVisible),
                TickLabelStyle = new FontStyle(fontSize, pressureColor),
                TitleStyle = new FontStyle(fontSize, pressureColor),
                DrawMinorGridLines = false,
                IsPrimaryAxis = false
            };

            // init data
            _oxygenData = new XyDataSeries<DateTime, double> { SeriesName = "Oxygen" };
            _temperatureData = new XyDataSeries<DateTime, double> { SeriesName = "Temperature" };
            _pressureData = new XyDataSeries<DateTime, double> { SeriesName = "Pressure" };

            // init series
            _oxygenSeries = new FastLineRenderableSeries
            {
                XAxisId = X_AXIS_ID,
                YAxisId = OXYGEN_AXIS_ID,
                DataSeries = _oxygenData,
                IsVisible = NativeControl.IsOxygenVisible,
                StrokeStyle = new SolidPenStyle(oxygenColor.ToSystemColor(), thickness)
            };
            _temperatureSeries = new FastLineRenderableSeries
            {
                XAxisId = X_AXIS_ID,
                YAxisId = TEMPERATURE_AXIS_ID,
                DataSeries = _temperatureData,
                IsVisible = NativeControl.IsTemperatureVisible,
                StrokeStyle = new SolidPenStyle(temperatureColor.ToSystemColor(), thickness)
            };
            _pressureSeries = new FastLineRenderableSeries
            {
                XAxisId = X_AXIS_ID,
                YAxisId = PRESSURE_AXIS_ID,
                DataSeries = _pressureData,
                IsVisible = NativeControl.IsPressureVisible,
                StrokeStyle = new SolidPenStyle(pressureColor.ToSystemColor(), thickness)
            };

            // add chart components to chart
            ChartSurface.XAxes.Add(dateAxis);
            ChartSurface.YAxes.Add(oxygenAxis);
            ChartSurface.YAxes.Add(temperatureAxis);
            ChartSurface.YAxes.Add(pressureAxis);
            ChartSurface.RenderableSeries.Add(_oxygenSeries);
            ChartSurface.RenderableSeries.Add(_temperatureSeries);
            ChartSurface.RenderableSeries.Add(_pressureSeries);

            // apply theme
            ChartSurface.Theme = Resource.Style.SciChart_Bright_Spark;

            // add modifiers
            if (NativeControl.IsZoomable)
            {
                ChartSurface.ChartModifiers = new ChartModifierCollection
                {
                    new ZoomPanModifier()
                    {
                        ClipModeX = ClipMode.None,
                        ClipModeY = ClipMode.None,
                        ZoomExtentsY = false
                    },
                    new PinchZoomModifier(){
                        Direction = Direction2D.XyDirection
                    },
                    new ZoomExtentsModifier
                    {
                        IsAnimated = true
                    }
                };
            }

            // load!
            AddView(ChartSurface);
        }

        internal void ReinitializeData()
        {
            ResetData();
            AddData(NativeControl.Data);
        }
        private void AddData(IEnumerable<MeasurementPoint> data)
        {
            if (data == null) return;

            var xValues = new List<DateTime>();
            var oxygenValues = new List<double>();
            var temperatureValues = new List<double>();
            var pressureValues = new List<double>();

            data.OrderBy(x => x.Timestamp)
                .ToList()
                .ForEach(x =>
                {
                    xValues.Add(x.Timestamp);
                    oxygenValues.Add(x.Value);
                    temperatureValues.Add(x.Temperature);
                    pressureValues.Add(x.Pressure);
                });

            _oxygenData.Append(xValues, oxygenValues);
            _temperatureData.Append(xValues, temperatureValues);
            _pressureData.Append(xValues, pressureValues);
        }
        private void RemoveData(int startingIndex, int count)
        {
            _oxygenData.RemoveRange(startingIndex, count);
            _temperatureData.RemoveRange(startingIndex, count);
            _pressureData.RemoveRange(startingIndex, count);
        }
        private void ResetData()
        {
            _oxygenData.Clear();
            _temperatureData.Clear();
            _pressureData.Clear();
        }

        private void UpdateAxisVisibility(string axisId, bool isVisible)
        {
            var axis = ChartSurface.YAxes.GetAxisById(axisId) as AxisCore;
            axis.Visibility = ConvertBoolToInt(isVisible);
            ChartSurface.RenderableSeries.Where(x => x.YAxisId == axisId).Cast<FastLineRenderableSeries>().ForEach(x => x.IsVisible = isVisible);
            ChartSurface.ZoomExtents();
        }

        private static int ConvertBoolToInt(bool boolVal)
            => boolVal ? (int)ViewStates.Visible : (int)ViewStates.Gone;
        #endregion

        #region Event Handles
        internal void OnDataCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    AddData(e.NewItems.Cast<MeasurementPoint>());
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    RemoveData(e.OldStartingIndex, e.OldItems.Count);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    ReinitializeData();
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                default:
                    throw new InvalidOperationException("The graph control doesn't support this operation");
            }
        }
        internal void OnCoreControlPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Native.MeasurementGraphView.DataProperty.PropertyName)
            {
                ReinitializeData();
            }
            else if (e.PropertyName == Native.MeasurementGraphView.IsOxygenVisibleProperty.PropertyName)
            {
                UpdateAxisVisibility(OXYGEN_AXIS_ID, (sender as Native.MeasurementGraphView).IsOxygenVisible);
            }
            else if (e.PropertyName == Native.MeasurementGraphView.IsTemperatureVisibleProperty.PropertyName)
            {
                UpdateAxisVisibility(TEMPERATURE_AXIS_ID, (sender as Native.MeasurementGraphView).IsTemperatureVisible);
            }
            else if (e.PropertyName == Native.MeasurementGraphView.IsPressureVisibleProperty.PropertyName)
            {
                UpdateAxisVisibility(PRESSURE_AXIS_ID, (sender as Native.MeasurementGraphView).IsPressureVisible);
            }
        }
        #endregion
    }
}