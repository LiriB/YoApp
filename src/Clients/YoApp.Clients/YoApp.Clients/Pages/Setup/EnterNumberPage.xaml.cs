﻿using Autofac;
using Xamarin.Forms;
using YoApp.Clients.Forms;
using YoApp.Clients.ViewModels.Setup;

namespace YoApp.Clients.Pages.Setup
{
    public partial class EnterNumberPage : ContentPage, IPageService
    {
        public EnterNumberPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);

            BindingContext = App.Container.Resolve<EnterNumberViewModel>(
                new TypedParameter(typeof(IPageService), this));

            SubmitButton.IsEnabled = false;
        }

        protected override void OnAppearing()
        {
            NumberEntry.Focus();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
