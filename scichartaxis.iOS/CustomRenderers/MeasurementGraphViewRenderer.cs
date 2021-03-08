using System;
using System.Collections.Generic;
using System.Linq;
using CoreAnimation;
using SciChart.iOS.Charting;
using scichartaxis.Data;
using scichartaxis.Native;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(scichartaxis.Native.MeasurementGraphView), typeof(scichartaxis.iOS.CustomRenderers.MeasurementGraphViewRenderer))]
namespace scichartaxis.iOS.CustomRenderers
{
    public class MeasurementGraphViewRenderer : ViewRenderer<Native.MeasurementGraphView, SCIChartSurface>
    {
        #region Constants
        private const string X_AXIS_ID = "xAxis";
        private const string OXYGEN_AXIS_ID = "oxygenAxis";
        private const string TEMPERATURE_AXIS_ID = "temperatureAxis";
        private const string PRESSURE_AXIS_ID = "pressureAxis";
        #endregion

        #region Fields
        private Native.MeasurementGraphView _native;

        private SCIChartSurface _chartSurface;
        private SCIFastLineRenderableSeries _oxygenSeries;
        private SCIFastLineRenderableSeries _temperatureSeries;
        private SCIFastLineRenderableSeries _pressureSeries;

        private XyDataSeries<DateTime, double> _oxygenData;
        private XyDataSeries<DateTime, double> _temperatureData;
        private XyDataSeries<DateTime, double> _pressureData;
        #endregion

        #region Protected Methods
        protected override void OnElementChanged(ElementChangedEventArgs<Native.MeasurementGraphView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                if (e.OldElement.Data != null)
                {
                    e.OldElement.Data.CollectionChanged -= OnDataCollectionChanged;
                }
                e.OldElement.PropertyChanged -= OnCoreControlPropertyChanged;
            }

            if (e.NewElement != null)
            {
                _native = e.NewElement;
                if (Control == null)
                {
                    InitializeGraph();
                }

                if (e.NewElement.Data != null)
                {
                    e.NewElement.Data.CollectionChanged += OnDataCollectionChanged;
                }
                e.NewElement.PropertyChanged += OnCoreControlPropertyChanged;

                ReinitializeData();
            }
        }
        #endregion

        #region Private Methods
        private void InitializeGraph()
        {
            // init shared resources
            var oxygenColor = UIColor.Blue;
            var temperatureColor = UIColor.Red;
            var pressureColor = UIColor.Green;

            // init surface
            _chartSurface = new SCIChartSurface()
            {
                RenderableSeriesAreaBorderStyle = new SCISolidPenStyle(UIColor.Black, 0)
            };

            // init axes
            var dateAxis = new SCIDateAxis()
            {
                AutoRange = _native.IsZoomable ? SCIAutoRange.Once : SCIAutoRange.Always,
                GrowBy = new SCIDoubleRange(0.05, 0.05),
                DrawMinorGridLines = false,
                TickLabelStyle = new SCIFontStyle(14, UIColor.Black),
                TitleStyle = new SCIFontStyle(14, UIColor.Black),
                AxisId = X_AXIS_ID
            };
            var oxygenAxis = new SCINumericAxis()
            {
                AutoRange = _native.IsZoomable ? SCIAutoRange.Once : SCIAutoRange.Always,
                AxisAlignment = SCIAxisAlignment.Left,
                GrowBy = new SCIDoubleRange(0.05, 0.05),
                AxisId = OXYGEN_AXIS_ID,
                AxisTitle = "Oxygen",
                IsVisible = _native.IsOxygenVisible,
                TickLabelStyle = new SCIFontStyle(14, oxygenColor),
                TitleStyle = new SCIFontStyle(14, oxygenColor),
                DrawMinorGridLines = false,
                IsPrimaryAxis = true
            };
            var temperatureAxis = new SCINumericAxis()
            {
                AutoRange = _native.IsZoomable ? SCIAutoRange.Once : SCIAutoRange.Always,
                AxisAlignment = SCIAxisAlignment.Right,
                GrowBy = new SCIDoubleRange(0.05, 0.05),
                AxisId = TEMPERATURE_AXIS_ID,
                AxisTitle = "Temperature",
                IsVisible = _native.IsTemperatureVisible,
                TickLabelStyle = new SCIFontStyle(14, temperatureColor),
                TitleStyle = new SCIFontStyle(14, temperatureColor),
                DrawMinorGridLines = false,
                IsPrimaryAxis = false
            };
            var pressureAxis = new SCINumericAxis()
            {
                AutoRange = _native.IsZoomable ? SCIAutoRange.Once : SCIAutoRange.Always,
                AxisAlignment = SCIAxisAlignment.Right,
                GrowBy = new SCIDoubleRange(0.05, 0.05),
                AxisId = PRESSURE_AXIS_ID,
                AxisTitle = "Pressure",
                IsVisible = _native.IsPressureVisible,
                TickLabelStyle = new SCIFontStyle(14, pressureColor),
                TitleStyle = new SCIFontStyle(14, pressureColor),
                DrawMinorGridLines = false,
                IsPrimaryAxis = false
            };

            // init data
            _oxygenData = new XyDataSeries<DateTime, double> { SeriesName = "Oxygen" };
            _temperatureData = new XyDataSeries<DateTime, double> { SeriesName = "Temperature" };
            _pressureData = new XyDataSeries<DateTime, double> { SeriesName = "Pressure" };

            // init series
            _oxygenSeries = new SCIFastLineRenderableSeries
            {
                XAxisId = X_AXIS_ID,
                YAxisId = OXYGEN_AXIS_ID,
                DataSeries = _oxygenData,
                IsVisible = _native.IsOxygenVisible,
                StrokeStyle = new SCISolidPenStyle(oxygenColor, 1.5f)
            };
            _temperatureSeries = new SCIFastLineRenderableSeries
            {
                XAxisId = X_AXIS_ID,
                YAxisId = TEMPERATURE_AXIS_ID,
                DataSeries = _temperatureData,
                IsVisible = _native.IsTemperatureVisible,
                StrokeStyle = new SCISolidPenStyle(temperatureColor, 1.5f)
            };
            _pressureSeries = new SCIFastLineRenderableSeries
            {
                XAxisId = X_AXIS_ID,
                YAxisId = PRESSURE_AXIS_ID,
                DataSeries = _pressureData,
                IsVisible = _native.IsPressureVisible,
                StrokeStyle = new SCISolidPenStyle(pressureColor, 1.5f)
            };

            // add chart components to chart
            _chartSurface.XAxes.Add(dateAxis);
            _chartSurface.YAxes.Add(oxygenAxis);
            _chartSurface.YAxes.Add(temperatureAxis);
            _chartSurface.YAxes.Add(pressureAxis);
            _chartSurface.RenderableSeries.Add(_oxygenSeries);
            _chartSurface.RenderableSeries.Add(_temperatureSeries);
            _chartSurface.RenderableSeries.Add(_pressureSeries);

            // apply theme
            SCIThemeManager.ApplyTheme(_chartSurface, SCIThemeManager.Bright_Spark);

            // add modifiers
            if (_native.IsZoomable)
            {
                _chartSurface.ChartModifiers = new SCIChartModifierCollection
                {
                    new SCIZoomPanModifier()
                    {
                        ClipModeX = SCIClipMode.None,
                        ClipModeY = SCIClipMode.None,
                        ZoomExtentsY = false
                    },
                    new SCIPinchZoomModifier(){
                        Direction = SCIDirection2D.XyDirection
                    },
                    new SCIZoomExtentsModifier
                    {
                        IsAnimated = true
                    }
                };
            }

            // load!
            SetNativeControl(_chartSurface);
        }

        private void ReinitializeData()
        {
            ResetData();
            AddData(_native.Data);
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
            _oxygenData.RemoveRangeAt(startingIndex, count);
            _temperatureData.RemoveRangeAt(startingIndex, count);
            _pressureData.RemoveRangeAt(startingIndex, count);
        }
        private void ResetData()
        {
            _oxygenData.Clear();
            _temperatureData.Clear();
            _pressureData.Clear();
        }

        private void UpdateAxisVisibility(string axisId, bool isVisible)
        {
            var axis = _chartSurface.YAxes.GetAxisById(axisId);
            axis.IsVisible = isVisible;
            _chartSurface.RenderableSeries.Where(x => x.YAxisId == axisId).ForEach(x => x.IsVisible = isVisible);
            _chartSurface.ZoomExtents();
        }
        #endregion

        #region Event Handles
        private void OnDataCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
        private void OnCoreControlPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MeasurementGraphView.DataProperty.PropertyName)
            {
                ReinitializeData();
            }
            else if (e.PropertyName == MeasurementGraphView.IsOxygenVisibleProperty.PropertyName)
            {
                UpdateAxisVisibility(OXYGEN_AXIS_ID, (sender as MeasurementGraphView).IsOxygenVisible);
            }
            else if (e.PropertyName == MeasurementGraphView.IsTemperatureVisibleProperty.PropertyName)
            {
                UpdateAxisVisibility(TEMPERATURE_AXIS_ID, (sender as MeasurementGraphView).IsTemperatureVisible);
            }
            else if (e.PropertyName == MeasurementGraphView.IsPressureVisibleProperty.PropertyName)
            {
                UpdateAxisVisibility(PRESSURE_AXIS_ID, (sender as MeasurementGraphView).IsPressureVisible);
            }
        }
        #endregion
    }
}