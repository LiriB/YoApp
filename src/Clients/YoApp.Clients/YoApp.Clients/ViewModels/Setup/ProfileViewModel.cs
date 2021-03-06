﻿using Plugin.Connectivity;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YoApp.Clients.Forms;
using YoApp.Clients.Manager;
using YoApp.Clients.Pages.Setup;

namespace YoApp.Clients.ViewModels.Setup
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string NickName
        {
            get => _appUserManager.User.Nickname;
            set => _appUserManager.User.Nickname = value;
        }

        public string StatusMessage
        {
            get => _appUserManager.User.Status;
            set => _appUserManager.User.Status = value;
        }

        public ICommand SubmitCommand { get; }
        public bool CanSubmit { get; private set; } = true;

        private readonly IPageService _pageService;
        private readonly IAppUserManager _appUserManager;

        public ProfileViewModel(IPageService pageService, IAppUserManager appUserManager)
        {
            _pageService = pageService;
            _appUserManager = appUserManager;

            SubmitCommand = new Command(async () => await UpdateAndPersistUser(),
                () => CanSubmit);
        }

        private async Task UpdateAndPersistUser()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await _pageService.Navigation.PushAsync(new CompletePage());
                return;
            }

            CanSubmit = false;

            await _appUserManager.SyncUpAsync();
            await _pageService.Navigation.PushAsync(new CompletePage());
        }

        public async Task FetchUserFromServer()
        {
            if (!CrossConnectivity.Current.IsConnected
                || !await _appUserManager.SyncDownAsync())
                return;

            OnPropertyChanged(nameof(NickName));
            OnPropertyChanged(nameof(StatusMessage));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
