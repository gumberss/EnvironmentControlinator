using EnvironmentControlinator.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using System.IO.Ports;
using Gma.System.MouseKeyHook;
using System.Runtime.InteropServices;
using EnvironmentControlinator.Models;

namespace EnvironmentControlinator.Controllers
{

    public class ChatController
    {
        TwitchClient client;
        private static IKeyboardMouseEvents m_events;

        private string username = "gumberslive";

        public ChatController()
        {
            //var a = new Call().Execute(@$"https://id.twitch.tv/oauth2/token?client_id=wwro3mdfw6gi2zt93ixwivmhop942y&client_secret=c5pcz1ak8zletzq81wfbux12lavfei&grant_type=client_credentials");
            //a.Wait();
 
            //var b = a.Result;

            //var c = JsonSerializer.Deserialize<TwitchLoginResponse>(b);

            // pegar user follow
            //var d = new Call()
            //    .Execute(
            //    @$"https://api.twitch.tv/helix/users?client_id=wwro3mdfw6gi2zt93ixwivmhop942y&authorization=oauth:nj810z1wfxrv1v7g3txzrha9v38uar");

            //d.Wait();


            userCommands = new Dictionary<string, UserCommands>(4000);

            ConnectionCredentials credentials = new ConnectionCredentials("Controlinator", 
                //"oauth:nj810z1wfxrv1v7g3txzrha9v38uar" Gumbbers
                "oauth:ltw7qnmqhgu6fotmcyeo3og7vamc4n"//GumberLive
                );// get on https://twitchapps.com/tmi/ 
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, username);

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            //client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;
            client.OnUserJoined += OnUserJoined;



            client.Connect();
        }

        private void OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            if (e.Username != "streamelements" && e.Username != username)
                client.SendMessage(e.Channel, $"Seja muito bem vindo {e.Username}! Digite %comandos para ver os comandos de mudança do ambiente!");
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            //client.SendMessage(e.Channel,e.);

        }

        private DateTime dateChanged = DateTime.MinValue;

        Dictionary<String, UserCommands> userCommands;

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.StartsWith("%"))
            {
                if (e.ChatMessage.Message == "%comandos")
                {
                    client.SendMessage(e.ChatMessage.Channel, "%twitter %perigo %atenção %tranquilo %arcoiris %morreu %susto1");
                    return;
                }

                if (e.ChatMessage.Message == "%twitter")
                {
                    client.SendMessage(e.ChatMessage.Channel, "Para enviar um texto em audio, crie um tweet no Twitter marcando o @gumberslive :)");
                    return;
                }

                if (dateChanged.AddSeconds(3) > DateTime.Now)
                {
                    client.SendMessage(e.ChatMessage.Channel, "Estamos trocando para a cor anterior, só um segundo :)");
                    return;
                }

                //client.TimeoutUser(e.ChatMessage. Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(30), "Bad word! 30 minute timeout!");

                if (e.ChatMessage.Message == "%perigo")
                    SendChatMessageToArduino(e.ChatMessage, "0");

                else if (e.ChatMessage.Message == "%atenção")
                    SendChatMessageToArduino(e.ChatMessage, "1"); 

                else if (e.ChatMessage.Message == "%tranquilo")
                    SendChatMessageToArduino(e.ChatMessage, "2");

                else if (e.ChatMessage.Message == "%arcoiris")
                    SendChatMessageToArduino(e.ChatMessage, "3");

                else if (e.ChatMessage.Message == "%morreu")
                    SendChatMessageToArduino(e.ChatMessage, "8");

                else if (e.ChatMessage.Message == "%susto1")
                    SendChatMessageToArduino(e.ChatMessage, "9");
            }
        }

        public void SendChatMessageToArduino(ChatMessage chatMessage, String message)
        {
            if (!userCommands.ContainsKey(chatMessage.Username))
                userCommands.Add(chatMessage.Username, new UserCommands());

            //if (userCommands[chatMessage.Username].collorChanges > UserCommands.MAX_COLLOR_CHANGES_PER_USER)
            //{
            //    client.SendMessage(chatMessage.Channel, $"{chatMessage.Username}, Você atingiu o máximo de {UserCommands.MAX_COLLOR_CHANGES_PER_USER} trocas de cores por hoje :(");
            //    return;
            //}

            if (userCommands[chatMessage.Username].lastCollorChanged.AddSeconds(7) > DateTime.Now)
            {
                client.SendMessage(chatMessage.Channel, $"{chatMessage.Username}, Cada usuário so pode trocar de cor 1 vez a cada 7 segundos");
                return;
            }

            //if (userCommands[chatMessage.Username].collorChanges > UserCommands.MAX_COLLOR_CHANGES_PER_USER / 2)
            //    client.SendMessage(chatMessage.Channel, $"{chatMessage.Username}, Você atingiu a metade das trocas de cores permitidas por hoje: {UserCommands.MAX_COLLOR_CHANGES_PER_USER / 2}!");

            userCommands[chatMessage.Username].collorChanges++;
            userCommands[chatMessage.Username].lastCollorChanged = DateTime.Now;

            dateChanged = DateTime.Now;
            ArduinoHandler.SendMessage(message);

            client.SendMessage(chatMessage.Channel, $"Como desejar {chatMessage.Username} :)");
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            //if (e.WhisperMessage.Username == "my_friend")
            //    client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            //if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
            //    client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            //else
            //    client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }
    }
}
