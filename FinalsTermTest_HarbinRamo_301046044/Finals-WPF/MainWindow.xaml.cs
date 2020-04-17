using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;
using Finals_WPF.Models;
using System.Text.RegularExpressions;
/// <summary>
/// ** Student Name     : Harbin Ramo
/// ** Student Number   : 301046044
/// ** Lab Assignment   : Finals Practicals
/// ** Date (MM/dd/yyy) : 04/17/2020
/// </summary>
namespace Finals_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public COVID19DeathsGlobalDBContext _context = new COVID19DeathsGlobalDBContext();

        public MainWindow()
        {
            InitializeComponent();
            this.UploadData();
        }

        public async void UploadData()
        {
            try
            {
                string _filePath = Environment.CurrentDirectory + "\\covid19DeathsGlobal.csv";
                Task<Covid19deaths> _myTask = Task.Run(() => this.StartAsync(_filePath));

                await Task.WhenAll(_myTask);

                if (_myTask.IsCompleted)
                {
                    var _covidList = _context.Covid19deaths.ToList();
                    
                    this.CovidDataGrid.ItemsSource = null;
                    this.CovidDataGrid.ItemsSource = _covidList;

                    MessageBox.Show("Successfully retrieved CSV data.");

                    var getCountry = from c in _covidList
                                     group c by c.CountryRegion into newGroup
                                     orderby newGroup.Key
                                     select new { CountryRegion = newGroup.Key };

                    this.CountryRegionComboBox.Items.Clear();
                    this.CountryRegionComboBox.Items.Add("-----");
                    foreach (var _getCountryName in getCountry)
                    {
                        this.CountryRegionComboBox.Items.Add(_getCountryName.CountryRegion);
                    }
                    this.CountryRegionComboBox.SelectedIndex = 0;
                }
            }
            catch(Exception ex)
            {

            }
        }

        Covid19deaths StartAsync(string _filePath)
        {
            var _myResult = new Covid19deaths();

            List<string> _getHeader = null;
            int progressCount = 0;

            using (StreamReader _readData = new StreamReader(_filePath))
            {
                _getHeader = new List<string>();

                while (true)
                {
                    string _readLine = _readData.ReadLine();
                    if (_readLine == null)
                    {
                        break;
                    }
                    else
                    {
                        if (_getHeader.Count <= 0)
                        {
                            _getHeader.AddRange(_readLine.Split(',').ToList<string>());
                        }
                        else
                        {
                            Regex _parseString = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                            string[] _getCurrentRow = _parseString.Split(_readLine);

                            int count = 0;
                            foreach (var _getItem in _getHeader)
                            {
                                switch (_getItem)
                                {
                                    case "Province/State":
                                    case "Country/Region":
                                    case "Lat":
                                    case "Long":
                                        count++;
                                        break;
                                    default:

                                        _context.AddRange(
                                                new Covid19deaths
                                                {
                                                    ProvinceState = _getCurrentRow[0],
                                                    CountryRegion = _getCurrentRow[1],
                                                    RecordDate = Convert.ToDateTime(_getItem + "20"),
                                                    DeathNumber = int.Parse(_getCurrentRow[count])
                                                }
                                            );

                                        IncrementProgress(progressCount);
                                        progressCount++;

                                        _context.SaveChanges();
                                        
                                        count++;
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            return _myResult;
        }

        public void IncrementProgress(int i)
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(() => IncrementProgress(i));
            }
            else
            {
                if (i > this.ProcessProgressBar.Maximum)
                {
                    this.ProcessProgressBar.Maximum = i;
                }
                this.ProcessProgressBar.Value += i;
                this.LoadingTextBox.Text = i.ToString();
            }
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Covid19deaths _newData = new Covid19deaths();
                _newData.CountryRegion = this.CountryRegionComboBox.SelectedItem.ToString();
                _newData.ProvinceState = this.ProvinceStateTextBox.Text;
                _newData.RecordDate = this.DateDatePicker.SelectedDate.Value.Date;
                _newData.DeathNumber = int.Parse(this.DeathsTextBox.Text);

                _context.Add(_newData);
                _context.SaveChanges();

                var _covidList = _context.Covid19deaths.ToList();
                this.CovidDataGrid.ItemsSource = _covidList;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Item already exist");
                return;
            }
        }

    }
}
