using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net.Http;
using System.Web;
using System.Windows;
using Microsoft.Win32;
using System.IO;

namespace receiptLUIS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            txtOutput.Text = "";
            txtInputData.Text = "";
            txtFilePath.Text = "";
            txtOutput.Text = "";
            Cursor = Cursors.Wait;
            btnSend.IsEnabled = false;
            string input = txtInput.Text;
            txtInputData.Text = input;
            txtOutput.Text = await MakeRequest(input);
            btnSend.IsEnabled = true;
            Cursor = Cursors.Arrow;
        }

        static async Task<string> MakeRequest(string input)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // This app ID is for a public sample app that recognizes requests to turn on and turn off lights
            var luisAppId = "b8e1ffb1-4882-4145-b57c-031f91b7e0f1";
            var subscriptionKey = "a4d85ab6d72a4d83bad5bac9a5496836";

            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // The "q" parameter contains the utterance to send to LUIS
            queryString["q"] = input;
            //turn on the left light
            // These optional request parameters are set to their default values
            queryString["timezoneOffset"] = "0";
            queryString["verbose"] = "false";
            queryString["spellCheck"] = "false";
            queryString["staging"] = "false";

            var uri = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/" + luisAppId + "?" + queryString;
            var response = await client.GetAsync(uri);

            var strResponseContent = await response.Content.ReadAsStringAsync();
            return strResponseContent.ToString();
        }

        private async void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            txtInput.Text = "";
            txtOutput.Text = "";
            txtInputData.Text = "";
            txtFilePath.Text = "";
            string result = "";
            string line;
            string lines = "";
            Stream myStream = null;
            OpenFileDialog openFD = new OpenFileDialog();
            //Limit file open dialog to open only txt files
            openFD.Filter = "Text files (*.txt)|*.txt";
            if (openFD.ShowDialog() == true)
            {
                try
                {
                    if ((myStream = openFD.OpenFile()) != null)
                    {
                        Cursor = Cursors.Wait;
                        btnBrowse.IsEnabled = false;
                        string filepath = openFD.InitialDirectory + openFD.FileName;
                            using (StreamReader sr = new StreamReader(filepath))
                            {
                            txtFilePath.Text = filepath;
                            //read the opened file line by line
                            while ((line = sr.ReadLine()) != null)
                            {
                                result += await MakeRequest(line) + "\n---------------------------\n";
                                lines += line + "\n";
                            }
                        }
                        txtInputData.Text = lines;
                        txtOutput.Text = result;
                        Cursor = Cursors.Arrow;
                        btnBrowse.IsEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
    }
}
