﻿using Autofac;
using Xamarin.Forms;
using YoApp.Clients.Core;
using YoApp.Clients.Forms;
using YoApp.Clients.ViewModels.Setup;

namespace YoApp.Clients.Pages.Setup
{
    public partial class WaitVerificationPage : ContentPage, IPageService
    {
        public WaitVerificationPage(string phoneNumber)
        {
            InitializeComponent();
            CodeEntry.Focus();
            BindingContext = App.Container.Resolve<VerificationViewModel>(
                new NamedParameter("phoneNumber", phoneNumber),
                new TypedParameter(typeof(IPageService), this));
        }
    }
}
