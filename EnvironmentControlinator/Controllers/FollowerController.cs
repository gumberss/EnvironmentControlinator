using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.Api.V5.Models.Subscriptions;


namespace EnvironmentControlinator.Controllers
{
    public class FollowerController
    {
        private static TwitchAPI api;

        public FollowerController()
        {
            api = new TwitchAPI();
            api.Settings.ClientId = "client_id";
            api.Settings.AccessToken = "access_token"; // App Secret is not an Accesstoken
            //api.Helix.EventSub.CreateEventSubSubscriptionAsync("","", null, "webhook", );
        }
    }
}
