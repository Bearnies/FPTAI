using System;
using System.IO;
using System.Collections.Generic;
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
using System.Net;
using System.Threading;

namespace FPTAI
{
    /// <summary>
    /// Interaction logic for TextToSpeech.xaml
    /// </summary>
    public partial class TextToSpeech : Window
    {
        public TextToSpeech()
        {
            InitializeComponent();
        }

        class ReturnData
        {
            public string async { get; set; }
            public string error { get; set; }
            public string message { get; set; }
            public string request_id { get; set; }
        }



        private void convertBtn_Click(object sender, RoutedEventArgs e)
        {
            Title = "Processing";

            string payload = textBoxContent.Text.ToString();

            string selectedVoice;
            ComboBoxItem typeItem = (ComboBoxItem)voiceSelection.SelectedItem;
            selectedVoice = typeItem.Name.ToString();

            String json = Task.Run(async () =>
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("api-key", "V6g9JmmqFy2KAff2ZxeId29RNYy12KUb");
                client.DefaultRequestHeaders.Add("speed", "1");
                client.DefaultRequestHeaders.Add("voice", selectedVoice);
                var response = await client.PostAsync("https://api.fpt.ai/hmi/tts/v5", new StringContent(payload));
                return await response.Content.ReadAsStringAsync();
            }).GetAwaiter().GetResult();

            var data = JsonConvert.DeserializeObject<ReturnData>(json);
            var folder = AppDomain.CurrentDomain.BaseDirectory;
            var filename = $"{folder}{Guid.NewGuid()}.mp3";

            Thread.Sleep(2000);

            using (var client = new WebClient())
            {
                client.DownloadFile(data.async, filename);
            }

            Title = "Download finished";

            var player = new MediaPlayer();
            player.Open(new Uri(filename, UriKind.Absolute));

            Title = "Playing audio file...";
            player.MediaEnded += Player_MediaEnded;

            player.Play();
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            Title = "Player ended";
        }
    }
}
