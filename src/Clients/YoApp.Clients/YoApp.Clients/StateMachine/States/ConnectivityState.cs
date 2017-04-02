﻿using System.Threading.Tasks;
using YoApp.Clients.Services;
using YoApp.Clients.Helpers.Extensions;

namespace YoApp.Clients.StateMachine.States
{
    public class ConnectivityState
    {
        public async Task HandleConnectivityState(bool isConnected)
        {
            if(isConnected)
            {
                if (AuthenticationService.AuthAccount != null)
                    await AuthenticationService.RequestToken(true);
            }
        }
    }
}
