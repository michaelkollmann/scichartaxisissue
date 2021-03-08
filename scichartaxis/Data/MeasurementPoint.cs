using System;
namespace scichartaxis.Data
{
    public class MeasurementPoint
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public double Temperature { get; set; }
        public double Pressure { get; set; }

        public MeasurementPoint()
        {
        }
    }
}
