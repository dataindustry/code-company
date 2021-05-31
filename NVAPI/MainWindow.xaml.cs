using SensorMonHTTP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using static SensorMonHTTP.HWiNFOWrapper;

namespace LaMetricEthermineMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HWiNFOWrapper wrapper = new();
        public Ethermine ethermine = new();
        public LaMetric lametric = new();
        ObservableCollection<Sensor> sensors = new();

        public MainWindow()
        {

            InitializeComponent();

            BindingOperations.EnableCollectionSynchronization(sensors, new Object());

            new Thread(ReadingSteiner).Start();

            this.EthermineWalletAddressTextBox.DataContext = ethermine;
            this.LaMetricURLTextBox.DataContext = lametric;
            this.LaMetricXAccessTokenTextBox.DataContext = lametric;

            this.sensorTreeView.ItemsSource = sensors;

        }

        private void PushButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(ethermine.WalletAddress);
            MessageBox.Show(lametric.URL);
            MessageBox.Show(lametric.XAccessToken);
        }

        private void ReadingSteiner()
        {

            while (true)
            {

                sensors.Clear();

                Tuple<List<_HWiNFO_SENSORS_SENSOR_ELEMENT>, List<_HWiNFO_SENSORS_READING_ELEMENT>> tuple = wrapper.Open();

                foreach (_HWiNFO_SENSORS_READING_ELEMENT item in tuple.Item2)
                {
                    Sensor sensor = new();
                    sensor.szLabelUser = item.szLabelUser;
                    sensor.SensorValue = item.Value.ToString("F2");
                    sensor.szUnit = item.szUnit;
                    sensors.Add(sensor);
                }

                Thread.Sleep(1000);
            }
        }

        private void FetchData() {
            
        }

        private void PushTOLaMetric() {

            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://192.168.1.58:4343/api/v1/dev/widget/update/com.lametric.bf28f997bc52b3fbb22209be3f7561b4/4"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("X-Access-Token", "M2QyN2ZiZmRiOWRjMWU4NDNhMjc1NTlhYjM1ZDQ3MWM2ZmE1N2EwMjFjMDBjNjMzNzIyN2M1ZTNkNDAzMGQ2Nw==");
                    request.Headers.TryAddWithoutValidation("Cache-Control", "no-cache");

                    request.Content = new StringContent("{\n    \"frames\": [\n        {\n            \"text\": \"1.0000\",\n            \"icon\": 44572,\n            \"index\": 0\n        },\n        {\n            \"text\": \"100.00\",\n            \"icon\": 44574,\n            \"index\": 1\n        },\n        {\n            \"text\": \"1 / 1\",\n            \"icon\": 44575,\n            \"index\": 2\n        },\n        {\n            \"text\": \"10000\",\n            \"icon\": 44568,\n            \"index\": 3\n        },\n        {\n            \"index\": 4,\n            \"chartData\": [\n                1,\n                2,\n                3,\n                4,\n                5,\n                6,\n                7,\n                8,\n                9,\n                10\n            ]\n        }\n    ]\n}");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    //  var response = await httpClient.SendAsync(request);
                }
            }

        }
    }

    public class Ethermine : INotifyPropertyChanged
    {
        private string _walletAddress;
        public string WalletAddress
        {
            set
            {
                _walletAddress = value;
                NotifyPropertyChanged(nameof(WalletAddress));
            }
            get { return _walletAddress; }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class LaMetric : INotifyPropertyChanged
    {
        private string _URL;
        public string URL
        {
            set
            {
                _URL = value;
                NotifyPropertyChanged(nameof(URL));
            }
            get { return _URL; }
        }

        private string _XAccessToken;
        public string XAccessToken
        {
            set
            {
                _XAccessToken = value;
                NotifyPropertyChanged(nameof(XAccessToken));
            }
            get { return _XAccessToken; }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Sensor : INotifyPropertyChanged
    {
        private string _szLabelUser;
        public string szLabelUser
        {
            set
            {
                _szLabelUser = value;
                NotifyPropertyChanged(nameof(szLabelUser));
            }
            get { return _szLabelUser; }
        }

        private string _sensorValue;
        public string SensorValue
        {
            set
            {
                _sensorValue = value;
                NotifyPropertyChanged(nameof(SensorValue));
            }
            get { return _sensorValue; }
        }

        private string _szUnit;
        public string szUnit
        {
            set
            {
                _szUnit = value;
                NotifyPropertyChanged(nameof(szUnit));
            }
            get { return _szUnit; }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
