using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace WPFGAME
{
    public class AlertHelper
    {
        public void ShowAlertText(UIElement alertText)
        {
            // Make the alert text visible
            alertText.Visibility = Visibility.Visible;

            // Set the initial opacity to 0 (in case it starts invisible)
            alertText.Opacity = 0;

            // Create a Storyboard to hold the animations
            Storyboard storyboard = new Storyboard();

            // Create a fade-in animation (0 to 1)
            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5) // Fade-in duration
            };
            Storyboard.SetTarget(fadeInAnimation, alertText);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));

            // Add fade-in animation to the storyboard
            storyboard.Children.Add(fadeInAnimation);

            // Start the fade-in animation
            storyboard.Begin();

            // Create a DispatcherTimer to wait for 5 seconds
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5) // Set the timer interval to 5 seconds
            };

            // When the timer ticks, start the fade-out animation
            timer.Tick += (sender, e) =>
            {
                // Stop the timer as we only need it once
                timer.Stop();

                // Create a fade-out animation (1 to 0)
                DoubleAnimation fadeOutAnimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.5) // Fade-out duration
                };
                Storyboard.SetTarget(fadeOutAnimation, alertText);
                Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));

                // Create a new storyboard for the fade-out animation
                Storyboard fadeOutStoryboard = new Storyboard();
                fadeOutStoryboard.Children.Add(fadeOutAnimation);

                // Begin the fade-out animation
                fadeOutStoryboard.Begin();

                // Optionally, hide the AlertText after the fade-out is complete
                fadeOutStoryboard.Completed += (s, args) =>
                {
                    alertText.Visibility = Visibility.Collapsed; // Hide the TextBlock after the animation completes
                };
            };

            // Start the timer
            timer.Start();
        }
    }
}