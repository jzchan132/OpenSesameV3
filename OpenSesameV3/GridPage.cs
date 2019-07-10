using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using TransparentWebViewXamForms;

namespace OpenSesameV3
{
	class GridPage : ContentPage
    {
		private readonly Color m_LIGHT_GRAY  = Color.FromRgb(242, 242, 242);
		private readonly Color m_LIGHT_GREEN = Color.FromRgb(146, 208,  80);
		private readonly Color m_DARK_BLUE   = Color.FromRgb( 37,  64,  97);
		private const int m_WIDE_SCREEN = 700;
		private int m_SETTINGS_FONT_SIZE = 24;
		private int m_SETTINGS_PADDING = 30;

		OpenSesameDatabase Database = null ;
		Entry UsernameEntry, PasswordEntry;
		Label VolumeValueLabel, ShakingForceValueLabel, ShakeCountValueLabel, PopupLabel;
		Image BackButtonImage, GrantAccessImage, HoldImage, LockImage, UnlockImage, SettingsImage, AboutImage;
		Slider ShakeCountSlider;
		string currentPage = "";
		int ScreenHeight = 0, ScreenWidth = 0;
		Grid PopupGrid, GrantAccessButtonGrid, HoldButtonGrid, LockButtonGrid, UnlockButtonGrid, LoginButtonGrid;
		Grid GroundPage = new Grid();  // GroundPage is for MaskGrid or PopupGridd
		Grid BasePage = new Grid();    // BasePage will be on top of GroundPage for e
		Grid MaskGrid = new Grid()
		{
			HorizontalOptions = LayoutOptions.FillAndExpand,
			VerticalOptions = LayoutOptions.FillAndExpand,
			BackgroundColor = Color.FromRgba(255, 255, 255, 128),
			RowSpacing = 0,
			ColumnSpacing = 0,
			RowDefinitions =
			{
				new RowDefinition { Height = GridLength.Star }
			},
			ColumnDefinitions =
			{
				new ColumnDefinition { Width = GridLength.Star}
			}
		};


        public GridPage(OpenSesameDatabase p_database, int height, int width)
		{
			this.Database = p_database;
			ScreenHeight = height;
			ScreenWidth  = width;
			if (ScreenWidth < m_WIDE_SCREEN)
			{
				m_SETTINGS_FONT_SIZE = 22;
				m_SETTINGS_PADDING = 10;
			}
		}

		private void CreateGroundPage()
		{
			GroundPage.Children.Clear();
			GroundPage = new Grid
			{
				BackgroundColor = m_DARK_BLUE,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				RowSpacing = 0,
				ColumnSpacing = 0,
				RowDefinitions =
				{
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				}
			};
		}

		protected override bool OnBackButtonPressed()
		{
			LOGMSG("OnBackButtonPressed currentPage=" + currentPage);
			if (currentPage.Equals("SettingsPage") || currentPage.Equals("AboutPage"))
			{
                DisplayHomePage();
                return true;
			}

			if (currentPage.Equals("LoginPage") || currentPage.Equals("HomePage"))
			{
				return false;
			}
			return true;
		}

        public void CreatePage()
        {
            // First time use this app, go to Login page
            if (Database.GetPropertyStrValue("sessionKey").Equals(""))
            {
                currentPage = "LoginPage";
                CreateLoginPage();
			}
            else
            {
                currentPage = "HomePage";
                CreateHomePage();
            }

			CreateGroundPage();
			GroundPage.Children.Add(BasePage, 0, 0);
			this.Content = GroundPage;
        }

        private void CreateHomePageTitle()
        {
            var TitleImage = new Image()
            {
                Source = Database.GetMessageStrValue("TitleImage"),
                HeightRequest = 66,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            AboutImage = new Image()
            {
                Source = "AboutUs.png",
                WidthRequest = 36,
                HeightRequest = 36,
                Margin = new Thickness(10, 0),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start
            };
            var AboutImageGestureRecognizer = new TapGestureRecognizer();
            AboutImageGestureRecognizer.Tapped += OnAboutImageTapped;
            AboutImage.GestureRecognizers.Add(AboutImageGestureRecognizer);

            SettingsImage = new Image()
            {
                Source = "Settings.png",
                WidthRequest = 36,
                HeightRequest = 36,
                Margin = new Thickness(10, 0),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End
            };
            var SettingsImageGestureRecognizer = new TapGestureRecognizer();
            SettingsImageGestureRecognizer.Tapped += OnSettingsImageTapped;
            SettingsImage.GestureRecognizers.Add(SettingsImageGestureRecognizer);

            BasePage.Children.Add(TitleImage,    0, 1, 0, 1);
            BasePage.Children.Add(AboutImage,    0, 1, 0, 1);
            BasePage.Children.Add(SettingsImage, 0, 1, 0, 1);
        }

        private void CreateUserHomePage()
        {
            BasePage.Children.Clear();
            BasePage = new Grid
            {
                BackgroundColor = m_DARK_BLUE,
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowSpacing = 0,
                ColumnSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(75, GridUnitType.Absolute) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                ColumnDefinitions =
                {
                   new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };

			var ButtonBaseGrid = new Grid
			{
				BackgroundColor = m_DARK_BLUE,
				VerticalOptions = LayoutOptions.FillAndExpand,
				RowSpacing = 0,
				ColumnSpacing = 0,
				Padding = new Thickness(30,0),
				RowDefinitions =
				{
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
				},
				ColumnDefinitions =
				{
				   new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				}
			};

			CreateHomePageTitle();

            // Create GrantAccess button
            GrantAccessImage = new Image() { Source = "UserButtonMask.png" };
            var GrantAccessLabel = new Label()
            {
                Text = Database.GetMessageStrValue("MainAccess"),
                FontSize = 30,
                TextColor = Color.White,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            GrantAccessButtonGrid = new Grid
            {
				HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = m_DARK_BLUE,
				RowSpacing = 0,
				ColumnSpacing = 0,
				RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(44, GridUnitType.Absolute) },
                    new RowDefinition { Height = new GridLength(12, GridUnitType.Absolute) },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto}
                }
            };
            var GrantAccessGestureRecognizer = new TapGestureRecognizer();
            GrantAccessGestureRecognizer.Tapped += OnGrantAccessButtonClicked;
            GrantAccessImage.GestureRecognizers.Add(GrantAccessGestureRecognizer);
            GrantAccessButtonGrid.Children.Add(GrantAccessLabel, 0, 1, 1, 3);
			GrantAccessButtonGrid.Children.Add(GrantAccessImage, 0, 1, 0, 3);

			// Create Hold buttonn
			HoldImage = new Image() { Source = "UserButtonMask.png" };

            var HoldLabel = new Label()
            {
                Text = Database.GetMessageStrValue("Hold"),
                FontSize = 30,
                TextColor = Color.White,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            HoldButtonGrid = new Grid
            {
                VerticalOptions = LayoutOptions.Start,
                RowSpacing = 0,
                ColumnSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(44, GridUnitType.Absolute) },
                    new RowDefinition { Height = new GridLength(12, GridUnitType.Absolute) },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto}
                }
            };
            var HoldGestureRecognizer = new TapGestureRecognizer();
            HoldGestureRecognizer.Tapped += OnHoldButtonClicked;
            HoldImage.GestureRecognizers.Add(HoldGestureRecognizer);
            HoldButtonGrid.Children.Add(HoldLabel, 0, 1, 1, 3);
			HoldButtonGrid.Children.Add(HoldImage, 0, 1, 0, 3);

			ButtonBaseGrid.Children.Add(GrantAccessButtonGrid, 0, 1, 0, 1);
            ButtonBaseGrid.Children.Add(HoldButtonGrid, 0, 1, 1, 2);

			BasePage.Children.Add(ButtonBaseGrid, 0, 1);
        }

        private void CreateAdminHomePage()
        {
            BasePage.Children.Clear();
            BasePage = new Grid
            {
                BackgroundColor = m_DARK_BLUE,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowSpacing = 10,
                ColumnSpacing = 0,
                Padding = new Thickness(0, 0, 0, 40),
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(75, GridUnitType.Absolute) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };

            CreateHomePageTitle();

            // Create GrantAccess button
            GrantAccessImage = new Image() { Source = "AdminButtonMask.png" };
            var GrantAccessLabel = new Label()
            {
                Text = Database.GetMessageStrValue("MainAccess"),
                FontSize = 20,
                TextColor = Color.White,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.CenterAndExpand
            };
            var GrantAccessLabelGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.Transparent,
                RowSpacing = 0,
                ColumnSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(48, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };
            GrantAccessLabelGrid.Children.Add(GrantAccessLabel, 1, 2, 0, 1);

            GrantAccessButtonGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = m_DARK_BLUE,
                RowSpacing = 0,
                ColumnSpacing = 0,
                RowDefinitions =
                {
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto}
                }
            };
            var GrantAccessGestureRecognizer = new TapGestureRecognizer();
            GrantAccessGestureRecognizer.Tapped += OnGrantAccessButtonClicked;
            GrantAccessImage.GestureRecognizers.Add(GrantAccessGestureRecognizer);
            GrantAccessButtonGrid.Children.Add(GrantAccessLabelGrid, 0, 1, 1, 2);
			GrantAccessButtonGrid.Children.Add(GrantAccessImage, 0, 1, 0, 3);

			// Create Hold buttonn
			HoldImage = new Image() { Source = "AdminButtonMask.png" };

            var HoldLabel = new Label()
            {
                Text = Database.GetMessageStrValue("Hold"),
                FontSize = 20,
                TextColor = Color.White,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            var HoldLabelGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                RowSpacing = 0,
                ColumnSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(48, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };
            HoldLabelGrid.Children.Add(HoldLabel, 1, 2, 0, 1);

            HoldButtonGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = m_DARK_BLUE,
                RowSpacing = 0,
                ColumnSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                ColumnDefinitions =
                {
					new ColumnDefinition { Width = GridLength.Auto}
				}
            };
            var HoldGestureRecognizer = new TapGestureRecognizer();
            HoldGestureRecognizer.Tapped += OnHoldButtonClicked;
            HoldImage.GestureRecognizers.Add(HoldGestureRecognizer);
            HoldButtonGrid.Children.Add(HoldLabelGrid, 0, 1, 1, 2);
			HoldButtonGrid.Children.Add(HoldImage, 0, 1, 0, 3);

			// Create Lock button
			LockImage = new Image() { Source = "AdminLockButtonMask.png" };
			var LockLabel = new Label()
            {
                Text = Database.GetMessageStrValue("Lock"),
                FontSize = 20,
                TextColor = Color.White,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            var LockLabelGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                RowSpacing = 0,
                ColumnSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(48, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };
            LockLabelGrid.Children.Add(LockLabel, 1, 2, 0, 1);

            LockButtonGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                RowSpacing = 0,
                ColumnSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                ColumnDefinitions =
                {
					new ColumnDefinition { Width = GridLength.Auto}
				}
            };
            var LockGestureRecognizer = new TapGestureRecognizer();
            LockGestureRecognizer.Tapped += OnLockButtonClicked;
            LockImage.GestureRecognizers.Add(LockGestureRecognizer);
            LockButtonGrid.Children.Add(LockLabelGrid, 0, 1, 1, 2);
			LockButtonGrid.Children.Add(LockImage, 0, 1, 0, 3);

			// Create Unlock button
			UnlockImage = new Image() { Source = "AdminUnlockButtonMask.png" };
            var UnlockLabel = new Label()
            {
                Text = Database.GetMessageStrValue("Unlock"),
                FontSize = 20,
                TextColor = Color.White,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            var UnlockLabelGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                RowSpacing = 0,
                ColumnSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(48, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };
            UnlockLabelGrid.Children.Add(UnlockLabel, 1, 2, 0, 1);

            UnlockButtonGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                RowSpacing = 0,
                ColumnSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                ColumnDefinitions =
                {
					new ColumnDefinition { Width = GridLength.Auto}
				}
            };
            var UnlockGestureRecognizer = new TapGestureRecognizer();
            UnlockGestureRecognizer.Tapped += OnUnlockButtonClicked;
            UnlockImage.GestureRecognizers.Add(UnlockGestureRecognizer);
            UnlockButtonGrid.Children.Add(UnlockLabelGrid, 0, 1, 1, 2);
			UnlockButtonGrid.Children.Add(UnlockImage, 0, 1, 0, 3);

			BasePage.Children.Add(GrantAccessButtonGrid, 0, 1, 1, 2);
            BasePage.Children.Add(HoldButtonGrid,        0, 1, 2, 3);
            BasePage.Children.Add(LockButtonGrid,        0, 1, 3, 4);
            BasePage.Children.Add(UnlockButtonGrid,      0, 1, 4, 5);
        }

        private void CreateHomePage()
		{
			if (!Database.GetPropertyStrValue("sessionKey").Equals(""))
			{
				if (Database.GetPropertyStrValue("role").Equals("admin"))
				{
                    CreateAdminHomePage();
				}
				else if (Database.GetPropertyStrValue("role").Equals("user"))
				{
                    CreateUserHomePage();
				}
			}
		}

		private Grid DisplayPopupGrid(string message)
		{
			LOGMSG("Show Popup Message " + message);
			PopupLabel = new Label()
			{
				Text = message,
				FontSize = 20,
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			PopupGrid = new Grid
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.Transparent,
				RowSpacing = 0,
				ColumnSpacing = 0,
				RowDefinitions =
				{
					new RowDefinition { Height = GridLength.Star},
					new RowDefinition { Height = GridLength.Star},
					new RowDefinition { Height = GridLength.Star}
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(10, GridUnitType.Absolute)},
					new ColumnDefinition { Width = GridLength.Star},
					new ColumnDefinition { Width = new GridLength(10, GridUnitType.Absolute)}
				}
			};
			var PopupCell = new Grid
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.FromRgba(0,0,0,230),
				RowSpacing = 10,
				ColumnSpacing = 10,
				Padding = new Thickness(50, 100, 50, 100)
			};
			PopupGrid.Children.Add(PopupCell, 1, 2, 1, 2);
			PopupGrid.Children.Add(PopupLabel, 1, 2, 1, 2);
			GroundPage.Children.Add(PopupGrid, 0, 0);

			this.Content = GroundPage;
			return PopupGrid;
		}
		private async Task WaitAndExecute(int milisec, Action actionToExecute)
		{
			await Task.Delay(milisec);
			actionToExecute();
		}
		void CleanPopupMessage()
		{
			LOGMSG("Clean Popup Message");
			CreatePage();
		}

		const string SUCCESS = "S01";
		async void OnGrantAccessButtonClicked(object sender, EventArgs e)
		{
			GrantAccessButtonGrid.BackgroundColor = m_LIGHT_GREEN;
			GroundPage.Children.Add(MaskGrid, 0, 0);
			await WaitAndExecute(10, () => GrantAccessButtonClicked());
        }
		async void GrantAccessButtonClicked()
        {
            string ErrorCode = DependencyService.Get<IOpenSesame>().GrantAccess();
            if (ErrorCode.Equals(SUCCESS))
                DisplayPopupGrid(Database.GetMessageStrValue("GrantAccessMessage"));
            else
                DisplayPopupGrid(Database.GetMessageStrValue(ErrorCode)); ;
			await WaitAndExecute(Database.GetPropertyIntValue("PopupTimeout"), () => CleanPopupMessage());
        }

        async void OnHoldButtonClicked(object sender, EventArgs e)
		{
			HoldButtonGrid.BackgroundColor = m_LIGHT_GREEN;
			GroundPage.Children.Add(MaskGrid, 0, 0);
			await WaitAndExecute(10, () => HoldButtonClicked());
		}
		async void HoldButtonClicked()
		{
			string ErrorCode = DependencyService.Get<IOpenSesame>().Hold();
			if (ErrorCode.Equals(SUCCESS))
				DisplayPopupGrid(Database.GetMessageStrValue("HoldMessage"));
			else
				DisplayPopupGrid(Database.GetMessageStrValue(ErrorCode));
			await WaitAndExecute(Database.GetPropertyIntValue("PopupTimeout"), () => CleanPopupMessage());
		}

		async void OnLockButtonClicked(object sender, EventArgs e)
		{
			LockButtonGrid.BackgroundColor = m_LIGHT_GREEN;
			GroundPage.Children.Add(MaskGrid, 0, 0);
			await WaitAndExecute(10, () => LockButtonClicked());
		}
		async void LockButtonClicked()
		{
			string ErrorCode = DependencyService.Get<IOpenSesame>().CardOnly();
			if (ErrorCode.Equals(SUCCESS))
				DisplayPopupGrid(Database.GetMessageStrValue("LockMessage"));
			else
				DisplayPopupGrid(Database.GetMessageStrValue(ErrorCode));
			await WaitAndExecute(Database.GetPropertyIntValue("PopupTimeout"), () => CleanPopupMessage());
		}

		async void OnUnlockButtonClicked(object sender, EventArgs e)
		{
			UnlockButtonGrid.BackgroundColor = m_LIGHT_GREEN;
			GroundPage.Children.Add(MaskGrid, 0, 0);
			await WaitAndExecute(10, () => UnlockButtonClicked());
		}
		async void UnlockButtonClicked()
		{
			string ErrorCode = DependencyService.Get<IOpenSesame>().Unlock();
			if (ErrorCode.Equals(SUCCESS))
				DisplayPopupGrid(Database.GetMessageStrValue("UnlockMessage"));
			else
				DisplayPopupGrid(Database.GetMessageStrValue(ErrorCode));
			await WaitAndExecute(Database.GetPropertyIntValue("PopupTimeout"), () => CleanPopupMessage());
		}

		private void CreateLoginPage()
		{
			BasePage.Children.Clear();
			BasePage = new Grid
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = m_DARK_BLUE,
				RowSpacing = 0,
				ColumnSpacing = 0,
				RowDefinitions =
				{
					new RowDefinition { Height = new GridLength(1,  GridUnitType.Star) },
					new RowDefinition { Height = new GridLength(1,  GridUnitType.Auto) },
					new RowDefinition { Height = new GridLength(1,  GridUnitType.Star) }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(30, GridUnitType.Absolute) },
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength(30, GridUnitType.Absolute) },
				}
			};

			var LoginGrid = new Grid
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
				BackgroundColor = Color.Transparent,
				RowSpacing = 10,
				ColumnSpacing = 0,
				RowDefinitions =
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = new GridLength(20, GridUnitType.Absolute) },
					new RowDefinition { Height = new GridLength(10, GridUnitType.Absolute) },
					new RowDefinition { Height = new GridLength(80, GridUnitType.Absolute) },
					new RowDefinition { Height = new GridLength(10, GridUnitType.Absolute) },
					new RowDefinition { Height = GridLength.Auto }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = GridLength.Star},
					new ColumnDefinition { Width = GridLength.Star}
				}
			};
            var TitleImage = new Image()
            {
                Source = Database.GetMessageStrValue("TitleImage")
            };

            UsernameEntry = new Entry()
			{
				Text = Database.GetPropertyStrValue("username"),
				FontSize = 30,
				TextColor = Color.Black,
				Placeholder = Database.GetMessageStrValue("Username"),
				PlaceholderColor = Color.FromRgb(75, 172, 198),
				BackgroundColor = Color.White
			};
			PasswordEntry = new Entry()
			{
				Text = "",
				IsPassword = true,
				FontSize = 30,
				TextColor = Color.Black,
				Placeholder = Database.GetMessageStrValue("Password"),
				PlaceholderColor = Color.FromRgb(75,172,198),
				BackgroundColor = Color.White
			};
			var ForgotPasswordLabel = new Label()
			{
				Text = Database.GetMessageStrValue("ForgotPassword"),
				FontSize = 14,
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center
			};
			// Create Login buttono
			var LoginButtonImage = new Image() { Source = "LoginButton.png" };
			var LoginButtonLabel = new Label()
			{
				Text = Database.GetMessageStrValue("Login"),
				FontSize = 20,
				TextColor = Color.White,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			LoginButtonGrid = new Grid
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				BackgroundColor = Color.FromRgb(75,172,198),
				RowSpacing = 0,
				ColumnSpacing = 0,
				RowDefinitions =
				{
					new RowDefinition { Height = GridLength.Auto }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = GridLength.Auto }
				}
			};
            var LoginGestureRecognizer = new TapGestureRecognizer();
            LoginGestureRecognizer.Tapped += OnLoginButtonClicked;
            LoginButtonImage.GestureRecognizers.Add(LoginGestureRecognizer);
			LoginButtonGrid.Children.Add(LoginButtonLabel, 0, 1, 0, 1);
			LoginButtonGrid.Children.Add(LoginButtonImage, 0, 1, 0, 1);

            var ENGImage = new Image()
			{
				Source = Database.GetPropertyStrValue("language").Equals("ENG") ? "ENGON.png" : "ENGOFF.png",
				WidthRequest = 72,
				HeightRequest = 72,
				VerticalOptions = LayoutOptions.Center
			};
			var ENGImageGestureRecognizer = new TapGestureRecognizer();
			ENGImageGestureRecognizer.Tapped += OnENGImageTapped;
			ENGImage.GestureRecognizers.Add(ENGImageGestureRecognizer);

			var ZHTImage = new Image()
			{
				Source = Database.GetPropertyStrValue("language").Equals("ZHT") ? "ZHTON.png" : "ZHTOFF.png",
				WidthRequest = 72,
				HeightRequest = 72,
				VerticalOptions = LayoutOptions.Center
			};
			var ZHTImageGestureRecognizer = new TapGestureRecognizer();
			ZHTImageGestureRecognizer.Tapped += OnZHTImageTapped;
			ZHTImage.GestureRecognizers.Add(ZHTImageGestureRecognizer);

			var ZHSImage = new Image()
			{
				Source = Database.GetPropertyStrValue("language").Equals("ZHS") ? "ZHSON.png" : "ZHSOFF.png",
				WidthRequest = 72,
				HeightRequest = 72,
				VerticalOptions = LayoutOptions.Center
			};
			var ZHSImageGestureRecognizer = new TapGestureRecognizer();
			ZHSImageGestureRecognizer.Tapped += OnZHSImageTapped;
			ZHSImage.GestureRecognizers.Add(ZHSImageGestureRecognizer);

			var LanguageGrid = new Grid()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				BackgroundColor = Color.Transparent,
				RowSpacing = 0,
				ColumnSpacing = 0,
				RowDefinitions =
				{
					new RowDefinition { Height = GridLength.Auto }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = GridLength.Star},
					new ColumnDefinition { Width = GridLength.Star},
					new ColumnDefinition { Width = GridLength.Star}
				}
			};
			LanguageGrid.Children.Add(ENGImage, 0, 1, 0, 1);
			LanguageGrid.Children.Add(ZHTImage, 1, 2, 0, 1);
			LanguageGrid.Children.Add(ZHSImage, 2, 3, 0, 1);

			LoginGrid.Children.Add(TitleImage,          0, 2, 0, 1);
			LoginGrid.Children.Add(UsernameEntry,       0, 2, 1, 2);
			LoginGrid.Children.Add(PasswordEntry,       0, 2, 2, 3);
			LoginGrid.Children.Add(ForgotPasswordLabel, 0, 2, 3, 4);
			LoginGrid.Children.Add(LoginButtonGrid,     0, 2, 5, 6);
			LoginGrid.Children.Add(LanguageGrid,        0, 2, 7, 8);

			BasePage.Children.Add(LoginGrid, 1, 2, 1, 2);
		}

        private void DisplayActivityIndicator()
        {
            LOGMSG("Display Activity Indicator");
			ActivityIndicator ai = new ActivityIndicator()
            {
                WidthRequest = 100,
                HeightRequest = 100,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                IsRunning = true
            };

			GroundPage.Children.Add(MaskGrid, 0, 0);
			GroundPage.Children.Add(ai,       0, 0);
            this.Content = GroundPage;
        }

        void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string ErrorCode = null;
            LOGMSG("OnLoginButtonClicked");
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;
            LOGMSG("Username=" + UsernameEntry.Text + " Password=" + PasswordEntry.Text);

            string ErrorTitle = "";
			LoginButtonGrid.BackgroundColor = m_LIGHT_GREEN;
			SetScreenDimensions(ScreenHeight, ScreenWidth);
			DisplayActivityIndicator();
            Task.Factory.StartNew(() => {
                ErrorCode = DependencyService.Get<IOpenSesame>().Authenticate(username, password);
            })
            .ContinueWith(task => {
                if (ErrorCode.StartsWith("S", StringComparison.Ordinal))
                    ErrorTitle = Database.GetMessageStrValue("SuccessfulCode") + " " + ErrorCode;
                else
                    ErrorTitle = Database.GetMessageStrValue("ErrorCode") + " " + ErrorCode;
                string ErrorDesc = Database.GetMessageStrValue(ErrorCode);
                DisplayAlert(ErrorTitle, ErrorDesc, Database.GetMessageStrValue("OK"));
                CreatePage();
            }, TaskScheduler.FromCurrentSynchronizationContext());
            LOGMSG("Finish OnLoginButtonClicked");
        }

        private void CreateSettingsPage()
		{
			BasePage.Children.Clear();
			BasePage = new Grid
			{
                BackgroundColor = m_DARK_BLUE,
                VerticalOptions = LayoutOptions.FillAndExpand,
				RowSpacing = 0,
				ColumnSpacing = 0,
				RowDefinitions =
				{
                    new RowDefinition { Height = new GridLength(75, GridUnitType.Absolute) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				}
			};

            var TitleLabel = new Label
            {
                Text = this.Database.GetMessageStrValue("Settings"),
                TextColor = Color.White,
                FontSize = 30,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            BackButtonImage = new Image()
            {
                Source = "BackButton.png",
                WidthRequest = 48,
                HeightRequest = 48,
                Margin = new Thickness(10, 0),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start
            };
            var BackButtonImageGestureRecognizer = new TapGestureRecognizer();
            BackButtonImageGestureRecognizer.Tapped += OnBackButtonImageTapped;
            BackButtonImage.GestureRecognizers.Add(BackButtonImageGestureRecognizer);

            BasePage.Children.Add(TitleLabel,      0, 1, 0, 1);
            BasePage.Children.Add(BackButtonImage, 0, 1, 0, 1);

            var SettingsGrid = new Grid
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.FromRgb(217, 217, 217), // Gray
				RowSpacing = 1,
				ColumnSpacing = 0,
				RowDefinitions =
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				}
			};
            var LanguageImage = new Image()
            {
                Source = "RoundButton.png",
                HeightRequest = 48,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };
            var LanguageLabel = new Label()
			{
				Text = Database.GetMessageStrValue("Language"),
				FontSize = m_SETTINGS_FONT_SIZE,
				TextColor = Color.Black,
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center
			};
            var LanguageImageLabel = new Label()
            {
                Text = Database.GetMessageStrValue(Database.GetPropertyStrValue("language")),
                FontSize = 24,
                TextColor = Color.White,
                Margin = new Thickness(15, 0, 15, 0),
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            var LanguageImageGrid = new Grid()
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
            LanguageImageGrid.Children.Add(LanguageImage,      0, 0);
            LanguageImageGrid.Children.Add(LanguageImageLabel, 0, 0);

            var LanguageGrid = new Grid
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				Padding = new Thickness(0, 20),
				RowDefinitions =
				{
					new RowDefinition { Height = GridLength.Auto }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				}
			};
            LanguageGrid.Children.Add(LanguageLabel,     0, 0);
            LanguageGrid.Children.Add(LanguageImageGrid, 0, 0);
            var LanguageGridGestureRecognizer = new TapGestureRecognizer();
            LanguageGridGestureRecognizer.Tapped += OnLanguageGridTapped;
            LanguageImageGrid.GestureRecognizers.Add(LanguageGridGestureRecognizer);

            var ProductionLabel = new Label()
            {
                Text = Database.GetMessageStrValue("ExecuteMode"),
				FontSize = m_SETTINGS_FONT_SIZE,
                TextColor = Color.Black,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            var ProductionImageSwitch = new ImageSwitch(Database, "ProductionMode", "Production", "Simulation", false);
            var ProductionGrid = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White,
                Padding = new Thickness(0, 20),
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };
            ProductionGrid.Children.Add(ProductionLabel, 0, 0);
            ProductionGrid.Children.Add(ProductionImageSwitch.GetImageSwitchGrid(), 0, 0);

			var VibrateLabel = new Label()
			{
				Text = Database.GetMessageStrValue("Vibration"),
				FontSize = m_SETTINGS_FONT_SIZE,
				TextColor = Color.Black,
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center
			};
			var VibrateImageSwitch = new ImageSwitch(Database, "Vibrate", "True", "False", false);
			var VibrateGrid = new Grid()
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				Padding = new Thickness(0, 20),
				RowDefinitions =
				{
					new RowDefinition { Height = GridLength.Auto }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				}
			};
			VibrateGrid.Children.Add(VibrateLabel, 0, 0);
			VibrateGrid.Children.Add(VibrateImageSwitch.GetImageSwitchGrid(), 0, 0);

			var ShakeLabel = new Label()
            {
                Text = Database.GetMessageStrValue("ShakeTitle"),
                FontSize = m_SETTINGS_FONT_SIZE,
                TextColor = Color.Black,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            var ShakeImageSwitch = new ImageSwitch(Database, "Shake", "True", "False", true);
            var ShakeGrid = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White,
                Padding = new Thickness(0, 20),
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };
            ShakeGrid.Children.Add(ShakeLabel, 0, 0);
            ShakeGrid.Children.Add(ShakeImageSwitch.GetImageSwitchGrid(), 0, 0);

            var ShakeCountLabel = new Label()
			{
				Text = Database.GetMessageStrValue("ShakeCount"),
				FontSize = m_SETTINGS_FONT_SIZE,
				TextColor = Color.Black,
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center
			};
			ShakeCountSlider = new Slider(Database.GetPropertyIntValue("MinShakeCount") * 1.0f, Database.GetPropertyIntValue("MaxShakeCount") * 1.0f, Database.GetPropertyIntValue("ShakeCount") * 1.0f)
			{
				HeightRequest = 50,
				WidthRequest = 500,
                HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			ShakeCountValueLabel = new Label()
			{
				Text = String.Format("{0:#}", Database.GetPropertyIntValue("ShakeCount")),
				FontSize = m_SETTINGS_FONT_SIZE,
				TextColor = Color.Black,
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center
			};
			var ShakeCountGrid = new Grid()
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				Padding = new Thickness(0, 20),
				RowDefinitions =
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				}
			};
			ShakeCountSlider.ValueChanged += OnShakeCountSliderValueChanged;
			ShakeCountGrid.Children.Add(ShakeCountLabel,      0, 1, 0, 1);
			ShakeCountGrid.Children.Add(ShakeCountValueLabel, 0, 1, 0, 1);
			ShakeCountGrid.Children.Add(ShakeCountSlider,     0, 1, 1, 2);

			var ShakingForceLabel = new Label()
			{
				Text = Database.GetMessageStrValue("ShakingForce"),
				FontSize = m_SETTINGS_FONT_SIZE,
				TextColor = Color.Black,
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center
			};
			Slider ShakingForceSlider = new Slider(Database.GetPropertyIntValue("MinGForce") * 0.1f, Database.GetPropertyIntValue("MaxGForce") * 0.1f, Database.GetPropertyIntValue("GForceThreshold") * 0.1f)
			{
				HeightRequest = 50,
				WidthRequest = 500,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			ShakingForceValueLabel = new Label()
			{
				Text = String.Format("{0:#.0G}", Database.GetPropertyIntValue("GForceThreshold") * 0.1f),
				FontSize = m_SETTINGS_FONT_SIZE,
				TextColor = Color.Black,
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center
			};
			var ShakingForceGrid = new Grid()
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				Padding = new Thickness(0, 20),
				RowDefinitions =
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				}
			};
			ShakingForceSlider.ValueChanged += OnShakingForceSliderValueChanged;
			ShakingForceGrid.Children.Add(ShakingForceLabel,      0, 1, 0, 1);
			ShakingForceGrid.Children.Add(ShakingForceValueLabel, 0, 1, 0, 1);
			ShakingForceGrid.Children.Add(ShakingForceSlider,     0, 1, 1, 2);

			var VolumeLabel = new Label()
			{
				Text = Database.GetMessageStrValue("Volume"),
				FontSize = m_SETTINGS_FONT_SIZE,
				TextColor = Color.Black,
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center
			};
			Slider VolumeSlider = new Slider(0, 100, Database.GetPropertyIntValue("Volume"))
			{
				HeightRequest = 50,
				WidthRequest = 500,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			VolumeValueLabel = new Label()
			{
				Text = Database.GetPropertyStrValue("Volume"),
				FontSize = m_SETTINGS_FONT_SIZE,
				TextColor = Color.Black,
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center
			};
			var VolumeGrid = new Grid()
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				Padding = new Thickness(0, 20),
				RowDefinitions =
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				}
			};
			VolumeSlider.ValueChanged += OnVolumeSliderValueChanged;
			VolumeGrid.Children.Add(VolumeLabel,      0, 1, 0, 1);
			VolumeGrid.Children.Add(VolumeValueLabel, 0, 1, 0, 1);
			VolumeGrid.Children.Add(VolumeSlider,     0, 1, 1, 2);

            var LogoutLabel = new Label()
			{
				Text = Database.GetMessageStrValue("Logout"),
				TextColor = Color.Red,
				FontSize = m_SETTINGS_FONT_SIZE,
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.Center
			};
			var LogoutGrid = new Grid()
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				Padding = new Thickness(0, 20),
				RowDefinitions =
				{
					new RowDefinition { Height = GridLength.Auto }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				}
			};
			LogoutGrid.Children.Add(LogoutLabel, 0, 0);
			var LogoutGridGestureRecognizer = new TapGestureRecognizer();
			LogoutGridGestureRecognizer.Tapped += OnLogoutGridTapped;
			LogoutGrid.GestureRecognizers.Add(LogoutGridGestureRecognizer);

			var ResetFactorySettingsLabel = new Label()
			{
				Text = Database.GetMessageStrValue("ResetFactorySettings"),
				TextColor = Color.Black,
				FontSize = m_SETTINGS_FONT_SIZE,
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.Center
			};
			var ResetFactorySettingsGrid = new Grid()
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				Padding = new Thickness(0, 20),
				RowDefinitions =
				{
					new RowDefinition { Height = GridLength.Auto }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				}
			};
			ResetFactorySettingsGrid.Children.Add(ResetFactorySettingsLabel, 0, 0);
			var ResetFactorySettingsGridGestureRecognizer = new TapGestureRecognizer();
			ResetFactorySettingsGridGestureRecognizer.Tapped += OnResetFactorySettingsGridTapped;
			ResetFactorySettingsGrid.GestureRecognizers.Add(ResetFactorySettingsGridGestureRecognizer);


			SettingsGrid.Children.Add(LanguageGrid,     0, 0);
			SettingsGrid.Children.Add(ProductionGrid,   0, 1);
			SettingsGrid.Children.Add(VibrateGrid,      0, 2);
			SettingsGrid.Children.Add(ShakeGrid,        0, 3);
			SettingsGrid.Children.Add(ShakeCountGrid,   0, 4);
			SettingsGrid.Children.Add(ShakingForceGrid, 0, 5);
			SettingsGrid.Children.Add(VolumeGrid,       0, 6);

            var ResetGrid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromRgb(217, 217, 217), // Gray
                RowSpacing = 1,
                ColumnSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };
            ResetGrid.Children.Add(LogoutGrid,               0, 0);
            ResetGrid.Children.Add(ResetFactorySettingsGrid, 0, 1);

            var BackgroundGrid = new Grid
			{
				BackgroundColor = m_LIGHT_GRAY,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				RowSpacing = 15,
				ColumnSpacing = 0,
				Padding = new Thickness(0, 15),
				RowDefinitions =
				{
					new RowDefinition { Height = new GridLength(1,  GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1,  GridUnitType.Auto) }
                },
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1,  GridUnitType.Star) }
				}
			};
			var WhiteBackgroundGrid0 = new Grid
			{
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				RowSpacing = 20,
				ColumnSpacing = 0,
				Padding = new Thickness(m_SETTINGS_PADDING, 0, m_SETTINGS_PADDING, 0),
				RowDefinitions =
				{
					new RowDefinition { Height = new GridLength(1,  GridUnitType.Star) }
                },
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1,  GridUnitType.Star) }
				}
			};
			WhiteBackgroundGrid0.Children.Add(SettingsGrid, 0, 0);

            var WhiteBackgroundGrid1 = new Grid
            {
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowSpacing = 20,
                ColumnSpacing = 0,
                Padding = new Thickness(m_SETTINGS_PADDING, 0, m_SETTINGS_PADDING, 0),
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1,  GridUnitType.Star) }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1,  GridUnitType.Star) }
                }
            };
            WhiteBackgroundGrid1.Children.Add(ResetGrid, 0, 0);

            BackgroundGrid.Children.Add(WhiteBackgroundGrid0, 0, 0);
            BackgroundGrid.Children.Add(WhiteBackgroundGrid1, 0, 1);
            var scrollview = new ScrollView
			{
				Content = BackgroundGrid
            };

			BasePage.Children.Add(scrollview, 0, 1, 1, 2);
		}

		async void OnSettingsImageTapped(object sender, EventArgs e)
		{
			LOGMSG("OnSettingsImageTapped");
			SettingsImage.Source = "SettingsHL.png";
			await WaitAndExecute(40, () => DisplaySettingsPage());
		}

		async void OnBackButtonImageTapped(object sender, EventArgs e)
		{
			LOGMSG("OnBackButtonImageTapped");
            BackButtonImage.Source = "BackButtonHL.png";
            await WaitAndExecute(40, () => CreatePage());
		}

		async void OnLanguageGridTapped(object sender, EventArgs e)
		{
			string[] Languages = {"ENG", "ZHT", "ZHS"};
			string[] LanguageTexts = { Database.GetMessageStrValue("ENG"), Database.GetMessageStrValue("ZHT"), Database.GetMessageStrValue("ZHS") };
			LOGMSG("OnLanguageButtonClicked " + Database.GetPropertyStrValue("language"));
			var languageText = await DisplayActionSheet(Database.GetMessageStrValue("SelectLanguage"), null, null, LanguageTexts);
			if (languageText != null)
			{
				LOGMSG("Language: " + languageText);

				foreach (string lang in Languages)
				{
					string langText = Database.GetMessageStrValue(lang);
					LOGMSG("Compare " + lang + " (" + languageText + " = " + langText + " ?)");
					if (languageText.Equals(langText) && !lang.Equals(Database.GetPropertyStrValue("language")))
					{
						DependencyService.Get<IOpenSesame>().SetLanguage(lang);
						Database.GetMessagesByLanguage(lang);
						CreateSettingsPage();
						this.Content = BasePage;
						break;
					}
				}
			}
		}

		void OnENGImageTapped(object sender, EventArgs e)
		{
			LOGMSG("OnENGImageTapped tapped");
			if (!Database.GetPropertyStrValue("language").Equals("ENG"))
			{
				DependencyService.Get<IOpenSesame>().SetLanguage("ENG");
				Database.GetMessagesByLanguage("ENG");
                DisplayLoginPage();
			}
		}

		void OnZHTImageTapped(object sender, EventArgs e)
		{
			LOGMSG("OnZHTImageTapped tapped");
			if (!Database.GetPropertyStrValue("language").Equals("ZHT"))
			{
				DependencyService.Get<IOpenSesame>().SetLanguage("ZHT");
				Database.GetMessagesByLanguage("ZHT");
                DisplayLoginPage();
            }
        }

		void OnZHSImageTapped(object sender, EventArgs e)
		{
			LOGMSG("OnZHSImageTapped tapped");
			if (!Database.GetPropertyStrValue("language").Equals("ZHS"))
			{
				DependencyService.Get<IOpenSesame>().SetLanguage("ZHS");
				Database.GetMessagesByLanguage("ZHS");
                DisplayLoginPage();
            }
        }

		void OnVolumeSliderValueChanged(object sender, ValueChangedEventArgs e)
		{
			int value = Convert.ToInt32(e.NewValue);
			LOGMSG("VolumeSlider is now " + e.NewValue + " " + value);
			Database.SaveProperties("Volume", value.ToString());
			VolumeValueLabel.Text = value.ToString();
			SetVolume(value * 0.01f);
		}

		void OnShakingForceSliderValueChanged(object sender, ValueChangedEventArgs e)
		{
			int value = Convert.ToInt32(e.NewValue * 10);
			String s = String.Format("{0:#.0}G", e.NewValue);
			LOGMSG("ShakingForceSlider is now " + e.NewValue + " " + value + " " + s);
			Database.SaveProperties("GForceThreshold", value.ToString());
			ShakingForceValueLabel.Text = s;
			SetShakingForceThreshold(value * 0.1f);
		}

		void OnShakeCountSliderValueChanged(object sender, ValueChangedEventArgs e)
		{
			double newStep = Math.Round(e.NewValue / 1.0f);
			int value = Convert.ToInt32(newStep);
			String s = String.Format("{0:#}", value);
			LOGMSG("ShakeCountSlider is now " + e.NewValue + " " + value + " " + s);
			Database.SaveProperties("ShakeCount", value.ToString());
			ShakeCountValueLabel.Text = s;
			ShakeCountSlider.Value = newStep;
			SetShakeCount(value);
		}

        async void OnLogoutGridTapped(object sender, EventArgs e)
        {
            LOGMSG("LogoutButton clicked");
			GroundPage.Children.Add(MaskGrid, 0, 0);
			await Task.Factory.StartNew(() =>
			{
				Database.SaveProperties("sessionKey", "");
				Database.SaveProperties("role", "");
			})
			.ContinueWith(Task => {
				DisplayLoginPage();
			}, TaskScheduler.FromCurrentSynchronizationContext());
            LOGMSG("Finish OnLogoutButtonClicked");
        }

        async void OnResetFactorySettingsGridTapped(object sender, EventArgs e)
		{
			LOGMSG("ResetFactorySettingsButton clicked");
			var answer = await DisplayAlert(Database.GetMessageStrValue("AreYouSure"), Database.GetMessageStrValue("ResetFactorySettingsDesc"), Database.GetMessageStrValue("OK"), Database.GetMessageStrValue("Cancel"));
			LOGMSG("answer = " + answer);
			if (answer)
			{
				DisplayActivityIndicator();
                await Task.Factory.StartNew(() => {
                    ResetFactorySettings();
                })
                .ContinueWith(task => {
                    DisplaySettingsPage();
                }, TaskScheduler.FromCurrentSynchronizationContext());
				LOGMSG("Finish OnResetFactorySettingsButtonClicked");
			}
		}

		private void ResetFactorySettings()
		{
			LOGMSG("ResetFactorySettings");
			Database.ResetFactorySettings();
			LOGMSG("LoadFactorySettings");
			DependencyService.Get<IOpenSesame>().LoadFactorySettings();
			var lang = Database.GetPropertyStrValue("language");
			DependencyService.Get<IOpenSesame>().SetLanguage(lang);
			Database.GetMessagesByLanguage(lang);
		}

		private void DisplaySettingsPage()
		{
			LOGMSG("DisplaySettingsPage");
            currentPage = "SettingsPage";
			CreateSettingsPage();
			CreateGroundPage();
			GroundPage.Children.Add(BasePage, 0, 0);
			this.Content = GroundPage;
		}

        private void DisplayLoginPage()
        {
            LOGMSG("DisplayLoginPage");
            currentPage = "LoginPage";
            CreateLoginPage();
			CreateGroundPage();
			GroundPage.Children.Add(BasePage, 0, 0);
			this.Content = GroundPage;
		}

        private void DisplayHomePage()
        {
            LOGMSG("DisplayHomePage");
            currentPage = "HomePage";
            CreateHomePage();
			CreateGroundPage();
			GroundPage.Children.Add(BasePage, 0, 0);
			this.Content = GroundPage;
        }

        private void DisplayAboutPage()
        {
            LOGMSG("DisplayAboutPage");
			currentPage = "AboutPage";
			CreateAboutPage();
			CreateGroundPage();
			GroundPage.Children.Add(BasePage, 0, 0);
			this.Content = GroundPage;
		}

        void CreateAboutPage()
        {
            BasePage.Children.Clear();

            BasePage = new Grid
            {
                BackgroundColor = m_DARK_BLUE,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowSpacing = 0,
                ColumnSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(75, GridUnitType.Absolute) },
                    new RowDefinition { Height = new GridLength(1,  GridUnitType.Star) },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };

            var TitleLabel = new Label
            {
                Text = this.Database.GetMessageStrValue("AboutTitle"),
                TextColor = Color.White,
                FontSize = 30,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            BackButtonImage = new Image()
            {
                Source = "BackButton.png",
                WidthRequest = 48,
                HeightRequest = 48,
                Margin = new Thickness(10, 0),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start
            };
            var BackButtonImageGestureRecognizer = new TapGestureRecognizer();
            BackButtonImageGestureRecognizer.Tapped += OnBackButtonImageTapped;
            BackButtonImage.GestureRecognizers.Add(BackButtonImageGestureRecognizer);

            BasePage.Children.Add(TitleLabel,      0, 1, 0, 1);
            BasePage.Children.Add(BackButtonImage, 0, 1, 0, 1);

			var BackgroundGrid = new Grid
			{
				BackgroundColor = m_LIGHT_GRAY,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				RowSpacing = 0,
				ColumnSpacing = 0,
				Padding = new Thickness(0, 15),
				RowDefinitions =
				{
					new RowDefinition { Height = new GridLength(1,  GridUnitType.Star) }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
				}
			};

			var browser = new TransparentWebView();
            var htmlSource = new HtmlWebViewSource();
            string html = Database.GetMessageStrValue("AboutHTML");
			LOGMSG(html);
            htmlSource.Html = html;
            browser.Source = htmlSource;
			BackgroundGrid.Children.Add(browser, 0, 1, 0, 1);
            BasePage.Children.Add(BackgroundGrid, 0, 1, 1, 2);

        }

        async void OnAboutImageTapped(object sender, EventArgs e)
		{
			AboutImage.Source = "AboutUsHL.png";
			await WaitAndExecute(10, () => DisplayAboutPage());
        }

		public void LOGMSG(string s)
		{
			DependencyService.Get<IOpenSesame>().LOGMSG(s);
		}
		public void SetVolume(float p_volume)
		{
			DependencyService.Get<IOpenSesame>().SetVolume(p_volume);
		}
		public void SetShakingForceThreshold(float p_gforce)
		{
			DependencyService.Get<IOpenSesame>().SetShakingForceThreshold(p_gforce);
		}
		public void SetShakeCount(int p_shakecount)
		{
			DependencyService.Get<IOpenSesame>().SetShakeCount(p_shakecount);
		}
		public void SetShakeEnabled(bool p_shakeEnabled)
		{
			DependencyService.Get<IOpenSesame>().SetShakeEnabled(p_shakeEnabled);
		}
		public void SetScreenDimensions(int height, int width)
		{
			DependencyService.Get<IOpenSesame>().SetScreenDimensions(height, width);
		}

	}
}
