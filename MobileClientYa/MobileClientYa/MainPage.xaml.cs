using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;

namespace MobileClientYa
{
    public partial class MainPage : ContentPage
    {
        HttpClient client;

        public MainPage()
        {
            InitializeComponent();
            client = new HttpClient();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IAudioRecord>().RecordAudio();
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            DependencyService.Get<IAudioRecord>().StopRecord();
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            var result = await DependencyService.Get<IAudioRecord>().SendFileToServer();

            if (String.IsNullOrEmpty(result))
            {
                await DisplayAlert("Ошибка", "Произошла ошибка при отправке файла", "Ok");
                return;
            }

            var dict = new Dictionary<string, string>
                {
                    { "filename", result }
                };

            var response = await client.PostAsync(
                @"http://195.135.252.148:5000/api",
                new FormUrlEncodedContent(dict));

            if(response.StatusCode.ToString() != "OK")
            {
                await DisplayAlert("Ошибка", "Произошла ошибка при отправке файла", "Ok");
                return;
            }

            await DisplayAlert("Успех", "Файл успешно отправлен", "Ok");

        }
    }
}
