using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace OpenSesameV3
{
	class ImageSwitch : Grid
    {
		OpenSesameDatabase Database = null ;
		Label ImageLabel;
		BoxView ImageSwitchBackgroundSliderBoxView, ImageSwitchForegroundSliderBoxView;
		Grid ImageSwitchGrid, ImageSwitchSliderBackgroundGrid;
        Image MaskImage;
        string m_key, m_trueValue, m_falseValue;
        bool m_callfunction;
        string m_TRUE, m_FALSE;
        bool m_mode;
		private readonly Color m_LIGHT_GREEN = Color.FromRgb(146, 208,  80);  // ON
		private readonly Color m_DARK_GRAY   = Color.FromRgb(166, 166, 166);  // OFF


        public ImageSwitch()
		{
		}
		public ImageSwitch(OpenSesameDatabase p_database, string p_key, string p_trueValue, string p_falseValue, bool p_callfunction)
		{
			Database = p_database;
            m_key = p_key;
            m_trueValue = p_trueValue;
            m_falseValue = p_falseValue;
            m_callfunction = p_callfunction;

            m_TRUE = Database.GetMessageStrValue("True");
            m_FALSE = Database.GetMessageStrValue("False");

            m_mode = Database.GetPropertyBoolValue(p_key, p_trueValue);
            MaskImage = new Image()
            {
                Source = "ImageSwitchMask.png",
                WidthRequest = 132,
                HeightRequest = 48,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            ImageSwitchGrid = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.End,
                BackgroundColor = Color.White,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };
            ImageLabel = new Label()
            {
                Text = m_mode ? m_TRUE : m_FALSE,
                FontSize = 24,
                TextColor = Color.White,
                Margin = new Thickness(15, 0, 15, 0),
                BackgroundColor = Color.Transparent,
                HorizontalOptions = m_mode ? LayoutOptions.Start : LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };
            ImageSwitchSliderBackgroundGrid = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = m_mode ? m_LIGHT_GREEN : m_DARK_GRAY,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };
            Image ImageSwithInnerButton = new Image()
            {
                Source = "ImageSwitchInnerButton.png",
                WidthRequest = 32,
                HeightRequest = 36,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            var ImageSwitchSliderForgroundGrid = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };
            ImageSwitchBackgroundSliderBoxView = new BoxView
            {
                Color = m_LIGHT_GREEN,
                WidthRequest = m_mode ? 125 : 1,
                HeightRequest = 48,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            ImageSwitchSliderBackgroundGrid.Children.Add(ImageSwitchBackgroundSliderBoxView, 0, 0);

            ImageSwitchForegroundSliderBoxView = new BoxView
            {
                Color = Color.Transparent,
                WidthRequest = m_mode ? 87 : 3,
                HeightRequest = 48,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            ImageSwitchSliderForgroundGrid.Children.Add(ImageSwitchForegroundSliderBoxView, 0, 0);
            ImageSwitchSliderForgroundGrid.Children.Add(ImageSwithInnerButton, 1, 0);

            ImageSwitchGrid.Children.Add(ImageSwitchSliderBackgroundGrid, 0, 0);
            ImageSwitchGrid.Children.Add(MaskImage, 0, 0);
            ImageSwitchGrid.Children.Add(ImageLabel, 0, 0);
            ImageSwitchGrid.Children.Add(ImageSwitchSliderForgroundGrid, 0, 0);

            var ImageSwitchGestureRecognizer = new TapGestureRecognizer();
            ImageSwitchGestureRecognizer.Tapped += OnImageSwitchToggled;
            ImageSwithInnerButton.GestureRecognizers.Add(ImageSwitchGestureRecognizer);
        }

        public Grid GetImageSwitchGrid()
        {
            return ImageSwitchGrid;
        }

        private async Task WaitAndExecute(int milisec, Action actionToExecute)
		{
			await Task.Delay(milisec);
			actionToExecute();
		}

		private void UpdateImageSwitchSliderBoxView(int b_width, int f_width)
		{
            ImageSwitchBackgroundSliderBoxView.WidthRequest = b_width;
            ImageSwitchForegroundSliderBoxView.WidthRequest = f_width;
        }
		private void FinalUpdate(int b_width, int f_width, Color color, string value, bool p_newMode)
		{
			UpdateImageSwitchSliderBoxView(b_width, f_width);
            ImageSwitchSliderBackgroundGrid.BackgroundColor = color;
            Database.SaveProperties(m_key, value);
            m_mode = p_newMode;
            if (m_callfunction) SetShakeEnabled(m_mode);
        }

        async private void OnImageSwitchToggled(object sender, EventArgs e)
		{
			if (m_mode)
			{
				ImageSwitchSliderBackgroundGrid.BackgroundColor = m_DARK_GRAY;
                int i = 87;
                while (i > 55)
                {
                    await WaitAndExecute(1, () => UpdateImageSwitchSliderBoxView(i+4,i));
                    i -= 10;
                }

                ImageLabel.Text = m_FALSE;
				ImageLabel.HorizontalOptions = LayoutOptions.End;

                while (i > 9)
                {
                    await WaitAndExecute(1, () => UpdateImageSwitchSliderBoxView(i+4,i));
                    i -= 5;
                }
                await WaitAndExecute(20, () => FinalUpdate(0, 2, m_DARK_GRAY, m_falseValue, false));
                await WaitAndExecute(20, () => FinalUpdate(0, 4, m_DARK_GRAY, m_falseValue, false));
                await WaitAndExecute(20, () => FinalUpdate(0, 3, m_DARK_GRAY, m_falseValue, false));
            }
            else
			{
				int i = 3;
				while (i < 55)
				{
					await WaitAndExecute(1, () => UpdateImageSwitchSliderBoxView(i+32,i));
					i += 10;
				}
				ImageLabel.Text = m_TRUE;
				ImageLabel.HorizontalOptions = LayoutOptions.Start;
				while (i < 80)
				{
					await WaitAndExecute(1, () => UpdateImageSwitchSliderBoxView(i+32,i));
					i += 5;
				}
				await WaitAndExecute(20, () => FinalUpdate(125, 88, m_LIGHT_GREEN, m_trueValue, true));
                await WaitAndExecute(20, () => FinalUpdate(125, 86, m_LIGHT_GREEN, m_trueValue, true));
                await WaitAndExecute(20, () => FinalUpdate(125, 87, m_LIGHT_GREEN, m_trueValue, true));
            }
        }

		private void SetShakeEnabled(bool p_shakeEnabled)
		{
			DependencyService.Get<IOpenSesame>().SetShakeEnabled(p_shakeEnabled);
		}
	}
}
