using System;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace VoiceInfra
{
    public class Synthesizer
    {
        private static SpeechConfig config;
        private static SpeechSynthesizer synthesizer;

        private static object _lock = new object();
        static Synthesizer()
        {
            config = SpeechConfig.FromSubscription("SUBSCRIPTION", "REGION");
            config.SpeechSynthesisLanguage = "pt-BR";

            synthesizer = new SpeechSynthesizer(config);
        }

        public static void SynthesizeAudio(String text)
        {
            lock (_lock)
            {
                synthesizer.SpeakTextAsync(text).Wait();
            }
        }
    }
}
