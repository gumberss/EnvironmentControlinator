using EnvironmentControlinator.Controllers;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using VoiceInfra;

namespace EnvironmentControlinator
{
    class Program
    {
        static void Main(string[] args)
        {

            new TweetController().Listen();

            //Console.ReadLine();

            //    return;

            new ChatController();

            var listenKeywordTask = ListenKeyPressed();

            listenKeywordTask.Wait();
            Console.ReadLine();
        }

        public static async Task<int> ListenKeyPressed()
        {
            new KeyPressedController().Start();

            return 0;
        }
    }

}