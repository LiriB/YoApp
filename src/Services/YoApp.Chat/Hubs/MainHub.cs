﻿using Microsoft.AspNet.SignalR;

namespace YoApp.Chat.Hubs
{
    [Authorize]
    public class MainHub : Hub
    {
        public MainHub()
        {
        }
    }
}
