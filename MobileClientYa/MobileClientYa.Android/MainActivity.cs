using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Media;
using System.IO;
using Xamarin.Forms;
using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage;
using AspNetCore.Yandex.ObjectStorage.Object.Responses;
using Android.Widget;
using System.Threading.Tasks;
using AndroidX.Core.App;
using Android;

[assembly: Dependency(typeof(MobileClientYa.Droid.RecordAudioTel))]
namespace MobileClientYa.Droid
{
    [Activity(Label = "MobileClientYa", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            ActivityCompat.RequestPermissions(this, new String[] {
                Manifest.Permission.RecordAudio,
                 Manifest.Permission.WriteExternalStorage,
                  Manifest.Permission.ReadExternalStorage,
                   Manifest.Permission.ManageExternalStorage,
                   Manifest.Permission.CaptureAudioOutput
            }, 0);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }


    public class RecordAudioTel : IAudioRecord
    {
        string filePath;
        string fileName;

        YandexStorageService storageService = new YandexStorageService(
                new YandexStorageOptions()
                {
                    BucketName = "audiotost",
                    AccessKey = "YouAccessKey",
                    SecretKey = "SecretKey"
                });

        protected MediaRecorder recorder;

        public void RecordAudio()
        {
            fileName = Guid.NewGuid().ToString() + ".ogg";
            filePath = Path.Combine(
            Android.OS.Environment.GetExternalStoragePublicDirectory(
                Android.OS.Environment.DirectoryDownloads).AbsolutePath
            , fileName);

            System.Diagnostics.Debug.WriteLine(filePath);

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                if (recorder == null)
                {
                    recorder = new MediaRecorder(); // Initial state.
                }

                recorder.Reset();
                recorder.SetAudioSource(AudioSource.Mic);
                recorder.SetOutputFormat(OutputFormat.Ogg);
                recorder.SetAudioEncoder(AudioEncoder.Opus);
                // Initialized state.
                recorder.SetOutputFile(filePath);
                // DataSourceConfigured state.
                recorder.Prepare(); // Prepared state
                recorder.Start(); // Recording state.
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.StackTrace);
            }
        }

        public async Task<string> SendFileToServer()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("TRY SEND");
                S3ObjectPutResponse response =
                await storageService.ObjectService.PutAsync(
                    File.ReadAllBytes(filePath),
                    fileName);


            }
            catch (Exception exep)
            {
                System.Diagnostics.Debug.WriteLine($"BAD -> {exep.Message}");
                return "";
            }

            System.Diagnostics.Debug.WriteLine("GOOD");

            return fileName;
        }

        public void StopRecord()
        {
            try
            {
                recorder.Stop();
                recorder.Release();
                System.Diagnostics.Debug.WriteLine("STOP RECORDING");
            }
            catch (Exception)
            {
            }
            finally
            {
                recorder.Dispose();
            }
        }
    }
}