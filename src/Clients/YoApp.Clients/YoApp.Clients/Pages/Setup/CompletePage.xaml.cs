﻿using Autofac;
using Xamarin.Forms;
using YoApp.Clients.Forms;
using YoApp.Clients.ViewModels.Setup;

namespace YoApp.Clients.Pages.Setup
{
    public partial class CompletePage : ContentPage, IPageService
    {
        private bool _canAnimate = true;

        public CompletePage()
        {
            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false);
            BindingContext = App.Container.Resolve<CompleteViewModel>(
                new TypedParameter(typeof(IPageService), this));
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if(_canAnimate)
                StartEntryAnimation();
        }

        private void StartEntryAnimation()
        {
            _canAnimate = false;

            Circle.Opacity = 0;
            GridBadge.Opacity = 0;
            BadgeBackground.Opacity = 0;
            GridBadge.Scale = 0;
            GridContainer.Opacity = 0;
            var animationController = new Animation();

            var badeBackgroundFade = new Animation(v => BadgeBackground.Opacity = v);
            var badgeFade = new Animation(v => GridBadge.Opacity = v, easing: Easing.CubicInOut);
            var badgeScale = new Animation(v => GridBadge.Scale = v, easing: Easing.BounceOut);
            var fadeCircle = new Animation(v => Circle.Opacity = v, 0.7, 0);
            var scaleCircle = new Animation(v => Circle.Scale = v, 0, 20, Easing.SinIn);
            var fadeLabelsContainer = new Animation(v => GridContainer.Opacity = v, easing: Easing.SinIn);

            animationController.Add(0.35, 0.75, badeBackgroundFade);
            animationController.Add(0.1, 0.45, badgeFade);
            animationController.Add(0.1, 0.4, badgeScale);
            animationController.Add(0.30, 0.90, fadeCircle);
            animationController.Add(0.30, 0.90, scaleCircle);
            animationController.Add(0.85, 1, fadeLabelsContainer);

            var badgeBackgroundRotation = new Animation(v => BadgeBackground.Rotation = v, 0, 4320);
            badgeBackgroundRotation.Commit(this, "RotaionAnimation", 16, 60000);

            animationController.Commit(this, "Animation", 
                16, 2000, null, 
                null, 
                () => true);

        }
    }
}
