using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scichartaxis.Data;
using Xamarin.Forms;

namespace scichartaxis
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<MeasurementPoint> _data;
        private Random _random;

        public MainPage()
        {
            InitializeComponent();

            _data = new ObservableCollection<MeasurementPoint>();
            Graph.Data = _data;

            _random = new Random();
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            _data.Add(new MeasurementPoint()
            {
                Timestamp = DateTime.Now,
                Value = _random.NextDouble(),
                Temperature = _random.NextDouble(),
                Pressure = _random.NextDouble(),
            });
        }
    }
}
