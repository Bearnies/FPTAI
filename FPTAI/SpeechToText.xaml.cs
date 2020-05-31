using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace FPTAI
{
    /// <summary>
    /// Interaction logic for SpeechToText.xaml
    /// </summary>
    public partial class SpeechToText : Window
    {
        public SpeechToText()
        {
            InitializeComponent();
        }

        class STTResult
        {
            public List<DictionaryEntry> hypotheses { get; set; }
        }

        class DictionaryEntry
        {
            public string utterance { get; set; }
        }

        private void convertBtn_Click(object sender, RoutedEventArgs e)
        {
            Title = "Processing";

            string filePath = textBoxContent.Text;

            var payload = File.ReadAllBytes(filePath);

            string json = Task.Run(async () =>
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("api-key", "V6g9JmmqFy2KAff2ZxeId29RNYy12KUb");

                var response = await client.PostAsync("https://api.fpt.ai/hmi/asr/general", new ByteArrayContent(payload));
                return await response.Content.ReadAsStringAsync();
            }).GetAwaiter().GetResult();

            try
            {
                var data = JsonConvert.DeserializeObject<STTResult>(json);
                MessageBox.Show(data.hypotheses[0].utterance);
            }
            catch
            {
                MessageBox.Show("Failed to load mp3 file !");
            }
        }

        private void selectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                textBoxContent.Text = openFileDlg.FileName;
            }
        }
    }
}
