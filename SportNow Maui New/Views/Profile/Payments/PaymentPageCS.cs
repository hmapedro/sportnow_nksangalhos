﻿using System.Diagnostics;
using SportNow.Model;
using SportNow.Services.Data.JSON;

namespace SportNow.Views.Profile.AllPayments
{
	public class PaymentPageCS : DefaultPage
	{

		protected override void OnAppearing()
		{
			if (App.isToPop == true)
			{
				App.isToPop = false;
				Navigation.PopAsync();
			}
			
		}

		protected override void OnDisappearing()
		{
			
		}
        double monthFeeValue;

        private Payment payment;

		private Microsoft.Maui.Controls.Grid gridPaymentOptions;

        

        public void initLayout()
		{
			Title = "PAGAMENTO";
		}


		public async void initSpecificLayout()
		{

			createPaymentOptions();

        }


		public void createPaymentOptions() { 

            Label selectPaymentModeLabel = new Label
			{
                FontFamily = "futuracondensedmedium",
                Text = "Escolhe o modo de pagamento pretendido:",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = App.normalTextColor,
				//LineBreakMode = LineBreakMode.NoWrap,
				FontSize = App.bigTitleFontSize
			};

			absoluteLayout.Add(selectPaymentModeLabel);
            absoluteLayout.SetLayoutBounds(selectPaymentModeLabel, new Rect(0, 10 * App.screenHeightAdapter, App.screenWidth - (20 * App.screenHeightAdapter), 80 * App.screenHeightAdapter));

			Image MBLogoImage = new Image
			{
				Source = "logomultibanco.png",
				MinimumHeightRequest = 115 * App.screenHeightAdapter,
				//WidthRequest = 100 * App.screenHeightAdapter, 
				HeightRequest = 115 * App.screenHeightAdapter,
				//BackgroundColor = Colors.Red,
			};

			var tapGestureRecognizerMB = new TapGestureRecognizer();
			tapGestureRecognizerMB.Tapped += OnMBButtonClicked;
			MBLogoImage.GestureRecognizers.Add(tapGestureRecognizerMB);


            Label TermsPaymentMBLabel = new Label
            {
                FontFamily = "futuracondensedmedium",
                Text = "Ao valor do pagamento é acrescido 1.7% e 0.22€ (+ IVA).", // \n Total a pagar:" + CalculateMBPayment(monthFeeValue) + "€",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = App.normalTextColor,
                FontSize = App.formLabelFontSize
            };
            absoluteLayout.Add(MBLogoImage);
            absoluteLayout.SetLayoutBounds(MBLogoImage, new Rect(0, 130 * App.screenHeightAdapter, App.screenWidth - 20 * App.screenHeightAdapter, 115 * App.screenHeightAdapter));
            absoluteLayout.Add(TermsPaymentMBLabel);
            absoluteLayout.SetLayoutBounds(TermsPaymentMBLabel, new Rect(0, 210 * App.screenHeightAdapter, App.screenWidth - 20 * App.screenHeightAdapter, 115 * App.screenHeightAdapter));

			
            Image MBWayLogoImage = new Image
			{
				Source = "logombway.png",
				//BackgroundColor = Colors.Green,
				//WidthRequest = 184 * App.screenHeightAdapter,
				MinimumHeightRequest = 115 * App.screenHeightAdapter,
				HeightRequest = 115 * App.screenHeightAdapter
			};

			var tapGestureRecognizerMBWay = new TapGestureRecognizer();
			tapGestureRecognizerMBWay.Tapped += OnMBWayButtonClicked;
			MBWayLogoImage.GestureRecognizers.Add(tapGestureRecognizerMBWay);

            Label TermsPaymentMBWayLabel = new Label
            {
                FontFamily = "futuracondensedmedium",
                Text = "Ao valor do pagamento é acrescido 0.7% e 0.07€ (+ IVA).",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = App.normalTextColor,
                FontSize = App.formLabelFontSize
            };

            absoluteLayout.Add(MBWayLogoImage);
            absoluteLayout.SetLayoutBounds(MBWayLogoImage, new Rect(0, 300 * App.screenHeightAdapter, App.screenWidth - 20 * App.screenHeightAdapter, 115 * App.screenHeightAdapter));
            absoluteLayout.Add(TermsPaymentMBWayLabel);
            absoluteLayout.SetLayoutBounds(TermsPaymentMBWayLabel, new Rect(0, 380 * App.screenHeightAdapter, App.screenWidth - 20 * App.screenHeightAdapter, 115 * App.screenHeightAdapter));


            Label ifthenpayLabel = new Label { FontFamily = "futuracondensedmedium", BackgroundColor = Colors.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = Colors.Blue, LineBreakMode = LineBreakMode.WordWrap };
            ifthenpayLabel.Text = "O valor acrescido corresponde à taxa de pagamento que a Ifthenpay cobra pela utilização e sincronização do seu serviço com a App.";

            absoluteLayout.Add(ifthenpayLabel);
            absoluteLayout.SetLayoutBounds(ifthenpayLabel, new Rect(0, 550 * App.screenHeightAdapter, App.screenWidth, 60 * App.screenHeightAdapter));

        }

        public PaymentPageCS(Payment payment)
		{
            Debug.WriteLine("PaymentPageCS payment.id=" + payment.id);
            this.payment = payment;

			//App.event_participation = event_participation;

			this.initLayout();
			this.initSpecificLayout();

		}


       /* public double CalculateMBPayment(double baseValue)
        {
            double percentIncrease = 1.7 / 100;
            double fixedIncrease = 0.22;

            double totalPayment = baseValue * (1 + percentIncrease) + fixedIncrease;

            return totalPayment;
        }

        public double CalculateMBWayPayment(double baseValue)
        {
            double percentIncrease = 0.7 / 100;
            double fixedIncrease = 0.07;

            double totalPayment = baseValue * (1 + percentIncrease) + fixedIncrease;

            return totalPayment;
        }*/


        async void OnMBButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new PaymentMBPageCS(this.payment));
		}


		async void OnMBWayButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new PaymentMBWayPageCS(this.payment));
		}

	}

  

}
