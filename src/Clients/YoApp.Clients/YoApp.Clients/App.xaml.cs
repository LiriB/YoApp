﻿using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Ioc;
using YoApp.Clients.Helpers;
using YoApp.Clients.Helpers.EventArgs;
using YoApp.Clients.Manager;
using YoApp.Clients.Persistence;

namespace YoApp.Clients
{
    public partial class App : Application
    {
        public static AppSettings Settings { get; private set; }
        public static IResolver Resolver { get; private set; }
        public static IResolver StorageResolver { get; private set; }

        private StateMachine.StateMachine _stateMachine;
        private bool _canResume;

        public App()
        {
            InitializeComponent();
            InitApp();
            SetMainPage();
        }

        private void InitApp()
        {
            StorageResolver = SetupStorageContainer().GetResolver();
            Resolver = SetupContainer().GetResolver();

            Settings = AppSettings.InitAppSettings();
            _stateMachine = new StateMachine.StateMachine();
        }

        private void SetMainPage()
        {
            if (Settings.SetupFinished || ResourceKeys.IsDebug)
                MainPage = new NavigationPage(new Pages.MainPage());
            else
                MainPage = new NavigationPage(new Pages.Setup.WelcomePage());
        }

        protected override async void OnStart()
        {
            await Task.Run(() =>
                {
                    MessagingCenter.Send(this, MessagingEvents.LifecycleChanged,
                        new LifecycleEventArgs(Lifecycle.Start));
                }).ConfigureAwait(false);
        }

        protected override async void OnSleep()
        {
            if (!_canResume)
                _canResume = true;

            await Task.Run(() =>
                {
                    MessagingCenter.Send(this, MessagingEvents.LifecycleChanged,
                        new LifecycleEventArgs(Lifecycle.Sleep));
                }).ConfigureAwait(false);
        }

        protected override async void OnResume()
        {
            //WORKAROUND: Prevent OnResume to be called at App start
            if (!_canResume)
                return;

            await Task.Run(() =>
                {
                    MessagingCenter.Send(this, MessagingEvents.LifecycleChanged,
                        new LifecycleEventArgs(Lifecycle.Resume));
                }).ConfigureAwait(false);

            //TODO: Check notifications and pending tasks
        }

        private IDependencyContainer SetupStorageContainer()
        {
            var container = new SimpleContainer();

            container.RegisterSingle<IKeyValueStore, AkavacheContext>();
            container.RegisterSingle<IRealmStore, RealmContext>();

            return container;
        }

        private IDependencyContainer SetupContainer()
        {
            var container = new XLabs.Ioc.SimpleContainer();

            container.RegisterSingle<IAppUserManager, AppUserManager>();
            container.RegisterSingle<IContactsManager, ContactsManager>();
            container.RegisterSingle<IFriendsManager, FriendsManager>();
            container.RegisterSingle<IChatManager, ChatManager>();
            container.Register<IVerificationManager>(typeof(VerificationManager));

            return container;
        }
    }
}
