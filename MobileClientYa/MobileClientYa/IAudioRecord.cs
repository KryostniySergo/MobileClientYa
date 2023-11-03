using System.Threading.Tasks;

namespace MobileClientYa
{
    public interface IAudioRecord
    {
        void RecordAudio();

        void StopRecord();

        Task<string> SendFileToServer();
    }
}
