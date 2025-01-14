﻿using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Diagnostics;
using SportNow.Views.Profile;
using SportNow.CustomViews;

namespace SportNow.Views
{
	public class QuotasPageCS : DefaultPage
	{

		protected async override void OnAppearing()
		{
			initSpecificLayout();
		}

		protected override void OnDisappearing()
		{
			this.CleanScreen();
		}

		private CollectionView collectionViewQuotas;

		private Member member;

		private Microsoft.Maui.Controls.Grid gridInactiveFee, gridActiveFee;

        RegisterButton activateButton;

		bool hasNextYearFeePayed = false;

		public void initLayout()
		{
			Title = "QUOTA";
		}

		public void CleanScreen()
		{
			Debug.Print("CleanScreen");
			//valida se os objetos já foram criados antes de os remover
			if (gridInactiveFee != null)
			{
				absoluteLayout.Remove(gridInactiveFee);
				gridInactiveFee = null;
			}
			if (gridActiveFee != null)
			{
				absoluteLayout.Remove(gridActiveFee);
				gridActiveFee = null;
			}
			

		}

		public async void initSpecificLayout()
		{
			member = App.member;

			var result = await GetCurrentFees(member);
			result = await GetNextYearFee(member);

			bool hasQuotaPayed = false;

			if (App.member.currentFee != null)
			{
				if ((App.member.currentFee.estado == "fechado") | (App.member.currentFee.estado == "recebido") | (App.member.currentFee.estado == "confirmado"))
				{
					hasQuotaPayed = true;
				}
			}

			if (App.member.nextPeriodFee != null)
			{
				if ((App.member.nextPeriodFee.estado == "fechado") | (App.member.nextPeriodFee.estado == "recebido") | (App.member.nextPeriodFee.estado == "confirmado"))
				{
					hasQuotaPayed = true;
					hasNextYearFeePayed = true;
				}
			}

			if (hasQuotaPayed) {
				createActiveFeeLayout();
			}
			else {
				createInactiveFeeLayout();
			}
		}

		public void createInactiveFeeLayout() {
			gridInactiveFee = new Microsoft.Maui.Controls.Grid { Padding = 10, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, RowSpacing = 10 * App.screenHeightAdapter };
			gridInactiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridInactiveFee.RowDefinitions.Add(new RowDefinition { Height = 100 });
			gridInactiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridInactiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridInactiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
			gridInactiveFee.RowDefinitions.Add(new RowDefinition { Height = 50 });
			gridInactiveFee.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //
            gridInactiveFee.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //
            gridInactiveFee.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 

			Label feeYearLabel = new Label
			{
                FontFamily = "futuracondensedmedium",
                Text = DateTime.Now.ToString("yyyy"),
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = App.normalTextColor,
				LineBreakMode = LineBreakMode.NoWrap,
				FontSize = App.bigTitleFontSize
			};

			Image akslLogoFee = new Image
			{
				Source = "company_logo.png",
				WidthRequest = 80,
                Opacity = 0.50
            };

            Image awikpLogoFee = new Image
            {
                Source = "logo_awikp.png",
                WidthRequest = 80,
                Opacity = 0.50
            };

            Image fnkpLogoFee = new Image
			{
				Source = "logo_fnkp.png",
				WidthRequest = 80,
                Opacity = 0.50
            };

			Label feeInactiveLabel = new Label
			{
                FontFamily = "futuracondensedmedium",
                Text = "Quota Inativa",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Colors.Red,
				LineBreakMode = LineBreakMode.NoWrap,
				FontSize = App.bigTitleFontSize
			};

			Label feeInactiveCommentLabel = new Label
			{
                FontFamily = "futuracondensedmedium",
                Text = "Atenção: Com as quotas inativas o aluno não poderá participar em eventos e não tem acesso a seguro desportivo em caso de lesão.",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = App.normalTextColor,
				FontSize = App.titleFontSize
			};

			activateButton = new RegisterButton("ATIVAR", App.screenWidth - 20 * App.screenWidthAdapter, 50 * App.screenHeightAdapter);
			activateButton.button.Clicked += OnActivateButtonClicked;


            gridInactiveFee.Add(feeYearLabel, 0, 0);
			Microsoft.Maui.Controls.Grid.SetColumnSpan(feeYearLabel, 3);

			gridInactiveFee.Add(fnkpLogoFee, 0, 1);
            gridInactiveFee.Add(awikpLogoFee, 1, 1);
            gridInactiveFee.Add(akslLogoFee, 2, 1);

			gridInactiveFee.Add(feeInactiveLabel, 0, 2);
			Microsoft.Maui.Controls.Grid.SetColumnSpan(feeInactiveLabel, 3);

			gridInactiveFee.Add(feeInactiveCommentLabel, 0, 3);
			Microsoft.Maui.Controls.Grid.SetColumnSpan(feeInactiveCommentLabel, 3);

			gridInactiveFee.Add(activateButton, 0, 5);
			Microsoft.Maui.Controls.Grid.SetColumnSpan(activateButton, 3);


			absoluteLayout.Add(gridInactiveFee);
            absoluteLayout.SetLayoutBounds(gridInactiveFee, new Rect(10 * App.screenWidthAdapter, 10 * App.screenHeightAdapter, App.screenWidth - 20 * App.screenWidthAdapter, App.screenHeight - 110 * App.screenHeightAdapter));

		}


		public void createActiveFeeLayout()
		{
			gridActiveFee = new Microsoft.Maui.Controls.Grid { Padding = 30, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
			gridActiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridActiveFee.RowDefinitions.Add(new RowDefinition { Height = 100 });
			gridActiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridActiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
			gridActiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridActiveFee.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto
			gridActiveFee.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 
            gridActiveFee.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto

            Label feeYearLabel = new Label
			{
				Text = DateTime.Now.ToString("yyyy"),
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = App.normalTextColor,
				LineBreakMode = LineBreakMode.NoWrap,
				FontSize = 50
			};

			if (hasNextYearFeePayed == true) 
			{
				feeYearLabel.Text = DateTime.Now.AddYears(1).ToString("yyyy");
			}
			else 
			{
				feeYearLabel.Text = DateTime.Now.ToString("yyyy");
			}

 			Image akslLogoFee = new Image
			{
				Source = "company_logo.png",
				WidthRequest = 80
			};

            Image awikpLogoFee = new Image
            {
                Source = "logo_awikp.png",
                WidthRequest = 80,
            };

            Image fnkpLogoFee = new Image
			{
				Source = "logo_fnkp.png",
				WidthRequest = 80

			};

			Label feeActiveLabel = new Label
			{
				Text = "Quotas Ativas",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = App.topColor,
				LineBreakMode = LineBreakMode.NoWrap,
				FontSize = App.bigTitleFontSize
			};

			Label feeActiveDueDateLabel = new Label
			{
				Text = "Válida até 31-12-"+DateTime.Now.ToString("yyyy"),
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = App.normalTextColor,
				LineBreakMode = LineBreakMode.NoWrap,
				FontSize = App.bigTitleFontSize
			};

			if (hasNextYearFeePayed == true) 
			{
				feeActiveDueDateLabel.Text = "Válida até 31-12-"+DateTime.Now.AddYears(1).ToString("yyyy");
			}
			else 
			{
				feeActiveDueDateLabel.Text = "Válida até 31-12-"+DateTime.Now.ToString("yyyy");
			}

			gridActiveFee.Add(feeYearLabel, 0, 0);
			Microsoft.Maui.Controls.Grid.SetColumnSpan(feeYearLabel, 3);

			gridActiveFee.Add(fnkpLogoFee, 0, 1);
			gridActiveFee.Add(awikpLogoFee, 1, 1);
            gridActiveFee.Add(akslLogoFee, 2, 1);

            gridActiveFee.Add(feeActiveLabel, 0, 2);
			Microsoft.Maui.Controls.Grid.SetColumnSpan(feeActiveLabel, 3);

			gridActiveFee.Add(feeActiveDueDateLabel, 0, 3);
			Microsoft.Maui.Controls.Grid.SetColumnSpan(feeActiveDueDateLabel, 3);

			absoluteLayout.Add(gridActiveFee);
            absoluteLayout.SetLayoutBounds(gridActiveFee, new Rect(0, 10 * App.screenHeightAdapter, App.screenWidth, App.screenHeight - 10 * App.screenHeightAdapter));

		}

		public QuotasPageCS()
		{
			this.initLayout();
		}

		async void OnPerfilButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new ProfileCS());
		}

		async void OnActivateButtonClicked(object sender, EventArgs e)
		{

			Debug.Print("App.member.member_type = "+App.member.member_type);
			showActivityIndicator();
			activateButton.IsEnabled = false;

			MemberManager memberManager = new MemberManager();


			if (App.member.currentFee is null) {

				var result_create = "0";
				
				result_create = await memberManager.CreateFee(App.member.id, App.member.member_type, DateTime.Now.ToString("yyyy"));
				if (result_create == "-1")
				{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = App.backgroundColor,
					BarTextColor = App.normalTextColor
				};
				}

				var result_get = await GetCurrentFees(member);
				if (result_create == "-1")
				{
					Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
					{
						BarBackgroundColor = App.backgroundColor,
						BarTextColor = App.normalTextColor
					};
				}
			}
			
			await Navigation.PushAsync(new QuotasPaymentPageCS(member));
			hideActivityIndicator();
		}
		

	

		async Task<int> GetCurrentFees(Member member)
		{
			Debug.WriteLine("GetCurrentFees");
			MemberManager memberManager = new MemberManager();

			var result = await memberManager.GetCurrentFees(member);
			if (result == -1)
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = App.backgroundColor,
					BarTextColor = App.normalTextColor
				};
				return result;
			}
			return result;
		}

		async Task<int> GetNextYearFee(Member member)
		{
			Debug.WriteLine("GetNextYearFee");
			MemberManager memberManager = new MemberManager();

			var result = await memberManager.GetNextYearFee(member);
			if (result == -1)
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = App.backgroundColor,
					BarTextColor = App.normalTextColor
				};
				return result;
			}
			return result;
		}

	}
}

