﻿using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using YoApp.Clients.Core;
using YoApp.Clients.Core.EventArgs;
using YoApp.Clients.StateMachine.States;
using YoApp.Clients.ViewModels.Setup;

namespace YoApp.Clients.StateMachine
{
    /// <summary>
    /// Manages all relevant states of the App.
    /// </summary>
    public class StateMachineController : IDisposable
    {
        private readonly LifeCycleState _lifeCycleState;
        private readonly SchedulerState _schedulerState;
        private readonly ConnectivityState _connectivityState;

        public StateMachineController(LifeCycleState lifeCycleState, SchedulerState schedulerState, 
            ConnectivityState connectivityState)
        {
            _lifeCycleState = lifeCycleState;
            _schedulerState = schedulerState;
            _connectivityState = connectivityState;
        }

        private async Task OnLifeCycleChanged(LifecycleEventArgs eventArgs)
        {
            _schedulerState.HandleState(eventArgs.State);
            await _lifeCycleState.HandleState(eventArgs.State);
        }

        private async Task OnConnectivityChanged(ConnectivityChangedEventArgs eventArgs)
        {
            await _connectivityState.HandleConnectivityState(eventArgs.IsConnected);
        }

        public void Start()
        {
            CrossConnectivity.Current.ConnectivityChanged +=
                async (s, e) => await OnConnectivityChanged(e);

            MessagingCenter.Subscribe<App, LifecycleEventArgs>(this, MessagingEvents.LifecycleChanged,
                async (s, e) => await OnLifeCycleChanged(e));

            if (!App.Settings.SetupFinished)
            {
                MessagingCenter.Subscribe<VerificationViewModel, LifecycleEventArgs>(this, MessagingEvents.LifecycleChanged,
                    async (s, e) => await OnLifeCycleChanged(e));
            }
        }

        public void Dispose()
        {
            CrossConnectivity.Current.ConnectivityChanged -=
                async (s, e) => await OnConnectivityChanged(e);

            MessagingCenter.Unsubscribe<VerificationViewModel>(this, MessagingEvents.UserCreated);
            MessagingCenter.Unsubscribe<App, LifecycleEventArgs>(this, MessagingEvents.LifecycleChanged);
        }
    }
}
