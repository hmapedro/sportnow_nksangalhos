﻿using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Diagnostics;
using SportNow.CustomViews;
using System.Text.RegularExpressions;
using System.Net;
using SportNow.Views.Profile.AllPayments;

namespace SportNow.Views.Profile
{
    public class ProfileCS : DefaultPage
	{

		protected async override void OnAppearing()
		{
            base.OnAppearing();
            var result = await GetCurrentFees(App.member);

			if (quotaImage == null)
			{
                quotaImage = new Image
                {
                    Aspect = Aspect.AspectFit
                };
            }

			quotaImage.Source = "iconquotasinativas.png";

			if (App.member != null)
			{ 
				if (App.member.currentFee != null)
				{
					if ((App.member.currentFee.estado == "fechado") | (App.member.currentFee.estado == "recebido") | (App.member.currentFee.estado == "confirmado"))
					{
                        quotaImage.Source = "iconquotasativas.png";
                    }
				}
			}
		}


		protected async override void OnDisappearing()
		{
			if (changeMember == false)
            {
				await UpdateMemberInfo();
			} 
			
		}

		Image quotaImage, objectivesImage;

		private ScrollView scrollView;

		MenuButton geralButton;
		MenuButton identificacaoButton;
		MenuButton moradaButton;
		MenuButton encEducacaoButton;


		Microsoft.Maui.Controls.StackLayout stackButtons;
		private Microsoft.Maui.Controls.Grid gridGeral;
		private Microsoft.Maui.Controls.Grid gridIdentificacao;
		private Microsoft.Maui.Controls.Grid gridMorada;
		private Microsoft.Maui.Controls.Grid gridFaturacao;
		private Microsoft.Maui.Controls.Grid gridButtons;

		FormValueEdit nameValue;
        FormValueEdit emailValue;
		FormValueEdit phoneValue;
		FormValueEdit addressValue;
		FormValueEdit cityValue;
		FormValueEdit postalcodeValue;
		FormValueEdit EncEducacao1NomeValue;
		FormValueEdit EncEducacao1PhoneValue;
		FormValueEdit EncEducacao1MailValue;
		FormValueEdit FaturaNomeValue;
		FormValueEdit FaturaMoradaValue;
		FormValueEdit FaturaCidadeValue;
        FormValueEdit CodPostalValue;
        FormValueEdit CidadeValue;
        FormValueEdit FaturaNIFValue;
        FormValue nifValue;
        FormValue cc_numberValue;
        FormValueEditDate birthdateValue;



        bool changeMember = false;

		bool enteringPage = true;

        RoundImage memberPhotoImage;
        Stream stream;


        int y_button_left = 0;
        int y_button_right = 0;


        public void initLayout()
		{
			Title = "PERFIL";

			var toolbarItem = new ToolbarItem
			{
				IconImageSource = "exit.png",
			};
			toolbarItem.Clicked += OnLogoutButtonClicked;
			ToolbarItems.Add(toolbarItem);
		}


		public async void initSpecificLayout()
		{
            LogManager logManager = new LogManager();
            _= await logManager.writeLog(App.original_member.id, App.member.id, "PROFILE VISIT", "Visit Profile Page");
            
            scrollView = new ScrollView { Orientation = ScrollOrientation.Vertical, MaximumHeightRequest = (App.screenHeight) - 100 - 350 * App.screenHeightAdapter, MaximumWidthRequest = App.screenWidth - 20 * App.screenWidthAdapter};

			absoluteLayout.Add(scrollView);
            absoluteLayout.SetLayoutBounds(scrollView, new Rect(0, 325 * App.screenHeightAdapter, App.screenWidth, (App.screenHeight) - 100 - 325 * App.screenHeightAdapter));


            int countStudents = App.original_member.students_count;

			CreatePhoto();			
			CreateGraduacao();
			CreateStackButtons();
			CreateGridGeral();
			CreateGridIdentificacao();
			CreateGridMorada();
			CreateGridFaturacao();
			CreateGridButtons();

            OnGeralButtonClicked(null, null);
        }

        public void CreatePhoto()
		{
            //memberPhotoImage = new RoundImage();

            memberPhotoImage = new RoundImage();

            WebResponse response;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Constants.images_URL + App.member.id + "_photo");
			Debug.Print(Constants.images_URL + App.member.id + "_photo");
            request.Method = "HEAD";
            bool exists;
            try
            {
                response = request.GetResponse();
                Debug.Print("response.Headers.GetType()= " + response.Headers.GetType());
                exists = true;
            }
            catch (Exception ex)
            {
                exists = false;
            }

            Debug.Print("Photo exists? = " + exists);

            if (exists)
            {

				memberPhotoImage.Source = new UriImageSource
				{
					Uri = new Uri(Constants.images_URL + App.member.id + "_photo"),
					CachingEnabled = false,
					//CacheValidity = new TimeSpan(0, 0, 0, 0)
				};
			}
            else
            {
                memberPhotoImage.Source = "iconadicionarfoto.png";
            }

            var memberPhotoImage_tap = new TapGestureRecognizer();
            memberPhotoImage_tap.Tapped += memberPhotoImageTappedAsync;
            memberPhotoImage.GestureRecognizers.Add(memberPhotoImage_tap);

			absoluteLayout.Add(memberPhotoImage);
            absoluteLayout.SetLayoutBounds(memberPhotoImage, new Rect((App.screenWidth/2) - (90 * App.screenHeightAdapter), 0, 180 * App.screenHeightAdapter, 180 * App.screenHeightAdapter));
        }

		public async Task<int> CreateQuotaButton()
		{

            quotaImage = new Image
            {
                Aspect = Aspect.AspectFit
            };

            var result = await GetCurrentFees(App.member);
            
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
				}
			}


			if (hasQuotaPayed == true)
			{
                quotaImage.Source = "iconquotasativas.png";
            }
			else
			{
                quotaImage.Source = "iconquotasinativas.png";
            }

            TapGestureRecognizer quotasImage_tapEvent = new TapGestureRecognizer();
            quotasImage_tapEvent.Tapped += OnQuotaButtonClicked;
            quotaImage.GestureRecognizers.Add(quotasImage_tapEvent);

			absoluteLayout.Add(quotaImage);
            absoluteLayout.SetLayoutBounds(quotaImage, new Rect((App.screenWidth) - (47.5 * App.screenHeightAdapter), y_button_right * App.screenHeightAdapter, 35 * App.screenHeightAdapter, 35 * App.screenHeightAdapter));

            Label quotasLabel = new Label
            {
                FontFamily = "futuracondensedmedium",
                Text = "Quota",
                TextColor = App.normalTextColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start,
                FontSize = App.smallTextFontSize
            };

			absoluteLayout.Add(quotasLabel);
            absoluteLayout.SetLayoutBounds(quotasLabel, new Rect((App.screenWidth) - (60 * App.screenHeightAdapter), (y_button_right+37) * App.screenHeightAdapter, 60 * App.screenHeightAdapter, 15 * App.screenHeightAdapter));

            y_button_right = y_button_right + 60;

            return 0;

        }

        public async Task<int> CreatePaymentsButton()
        {
            PaymentManager paymentManager = new PaymentManager();
            App.member.payments = await paymentManager.GetAllPayments_byUserId(App.member.id);

            if ((App.member.payments != null) & (App.member.payments.Count>0))
            {

                Image paymentsImage = new Image
                {
                    Aspect = Aspect.AspectFit
                };

                paymentsImage.Source = "iconconsentimentos.png";
                TapGestureRecognizer paymentsImage_tapEvent = new TapGestureRecognizer();
                paymentsImage_tapEvent.Tapped += OnPaymentsButtonClicked;
                paymentsImage.GestureRecognizers.Add(paymentsImage_tapEvent);

                absoluteLayout.Add(paymentsImage);
                absoluteLayout.SetLayoutBounds(paymentsImage, new Rect((App.screenWidth) - (47.5 * App.screenHeightAdapter), y_button_right * App.screenHeightAdapter, 35 * App.screenHeightAdapter, 35 * App.screenHeightAdapter));

                Label pagamentosLabel = new Label
                {
                    FontFamily = "futuracondensedmedium",
                    Text = "Pagamentos",
                    TextColor = App.normalTextColor,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Start,
                    FontSize = App.smallTextFontSize
                };

                TapGestureRecognizer pagamentosLabel_tapEvent = new TapGestureRecognizer();
                pagamentosLabel_tapEvent.Tapped += OnPaymentsButtonClicked;
                pagamentosLabel.GestureRecognizers.Add(pagamentosLabel_tapEvent);

                absoluteLayout.Add(pagamentosLabel);
                absoluteLayout.SetLayoutBounds(pagamentosLabel, new Rect((App.screenWidth) - (60 * App.screenHeightAdapter), (y_button_right + 37) * App.screenHeightAdapter, 60 * App.screenHeightAdapter, 15 * App.screenHeightAdapter));

                y_button_right = y_button_right + 60;
                return 1;
            }
            else
            {
                return 0;
            }
        }


        public async Task<int> CreateMedicalExamButton()
        {
            MemberManager memberManager = new MemberManager();
            App.member.medicalExams = await memberManager.Get_MedicalExam_byUserId(App.member.id);

            Image medicalExamsImage = new Image
            {
                Aspect = Aspect.AspectFit
            };

            
            TapGestureRecognizer medicalExamsImage_tapEvent = new TapGestureRecognizer();
            medicalExamsImage_tapEvent.Tapped += OnMedicalExamButtonClicked;
            medicalExamsImage.GestureRecognizers.Add(medicalExamsImage_tapEvent);

            absoluteLayout.Add(medicalExamsImage);
            absoluteLayout.SetLayoutBounds(medicalExamsImage, new Rect((12.5 * App.screenHeightAdapter), y_button_left * App.screenHeightAdapter, 35 * App.screenHeightAdapter, 35 * App.screenHeightAdapter));

            Label medicalExamsLabel = new Label
            {
                FontFamily = "futuracondensedmedium",
                Text = "Exame Médico",
                TextColor = App.normalTextColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start,
                FontSize = App.smallTextFontSize
            };

            TapGestureRecognizer medicalExamsLabel_tapEvent = new TapGestureRecognizer();
            medicalExamsLabel_tapEvent.Tapped += OnMedicalExamButtonClicked;
            medicalExamsLabel.GestureRecognizers.Add(medicalExamsLabel_tapEvent);

            absoluteLayout.Add(medicalExamsLabel);
            absoluteLayout.SetLayoutBounds(medicalExamsLabel, new Rect(0, (y_button_left + 37) * App.screenHeightAdapter, 60 * App.screenHeightAdapter, 15 * App.screenHeightAdapter));

            y_button_left = y_button_left + 60;

            if ((App.member.medicalExams == null) & (App.member.medicalExams.Count == 0))
            {
                medicalExamsImage.Source = "emd_vermelho.png";
                return 1;
            }
            else
            {
                bool isExpired = true;
                bool isAlmostExpired = false;
                foreach (MedicalExam medicalExam in App.member.medicalExams)
                {
                    if (medicalExam.status == "Aprovado")
                    {
                        isExpired = false;
                        DateTime currentTime = DateTime.Now.Date;
                        DateTime expireDate_datetime = DateTime.Parse(medicalExam.expireDate).Date;
                        Debug.Print("(expireDate_datetime - currentTime).Days = " + (expireDate_datetime - currentTime).Days);
                        if ((expireDate_datetime - currentTime).Days < 30)
                        {
                            isAlmostExpired = true;
                        }
                    }
                }

                if (isExpired)
                {
                    medicalExamsImage.Source = "emd_vermelho.png";
                }
                else if (isAlmostExpired)
                {
                    medicalExamsImage.Source = "emd_amarelo.png";
                }
                else
                {
                    medicalExamsImage.Source = "emd_verde.png";
                }

                return 0;
            }
        }

        public async void CreateObjectivesButton()
        {


			objectivesImage = new Image
            {
                Aspect = Aspect.AspectFit
            };

            objectivesImage.Source = "iconexpectativas.png";

            TapGestureRecognizer objectivesImage_tapEvent = new TapGestureRecognizer();
            objectivesImage_tapEvent.Tapped += OnObjectivesButtonClicked;
            objectivesImage.GestureRecognizers.Add(objectivesImage_tapEvent);

			absoluteLayout.Add(objectivesImage);
            absoluteLayout.SetLayoutBounds(objectivesImage, new Rect((App.screenWidth) - (47.5 * App.screenHeightAdapter), y_button_right * App.screenHeightAdapter, 35 * App.screenHeightAdapter, 35 * App.screenHeightAdapter));


            Label objectivesLabel = new Label
            {
                FontFamily = "futuracondensedmedium",
                Text = "Expectativas",
                TextColor = App.normalTextColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start,
                FontSize = App.smallTextFontSize
            };

			absoluteLayout.Add(objectivesLabel);
            absoluteLayout.SetLayoutBounds(objectivesLabel, new Rect((App.screenWidth) - (60 * App.screenHeightAdapter), (y_button_right + 37) * App.screenHeightAdapter, 60 * App.screenHeightAdapter, 15 * App.screenHeightAdapter));
        
            y_button_right = y_button_right + 60;


        }


        public async Task<int> CreateGraduacao()
		{
			string gradeBeltFileName = "belt_" + App.member.grade.ToLower() + ".png";

			Image gradeBeltImage = new Image
			{
				Source = gradeBeltFileName
			};
            var tapGestureRecognizer_graduacaoFrame = new TapGestureRecognizer();
            tapGestureRecognizer_graduacaoFrame.Tapped += async (s, e) => {
                await Navigation.PushAsync(new myGradesPageCS("MinhasGraduaçoes"));
            };
            gradeBeltImage.GestureRecognizers.Add(tapGestureRecognizer_graduacaoFrame);


			absoluteLayout.Add(gradeBeltImage);
			absoluteLayout.SetLayoutBounds(gradeBeltImage, new Rect((App.screenWidth/2) - (50 * App.screenHeightAdapter), 185 * App.screenHeightAdapter, 100 * App.screenHeightAdapter, 40 * App.screenHeightAdapter));

            Label gradeLabel = new Label
			{
                FontFamily = "futuracondensedmedium",
                Text = Constants.grades[App.member.grade],
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = App.normalTextColor,
				LineBreakMode = LineBreakMode.NoWrap,
				FontSize = App.itemTitleFontSize
			};
			absoluteLayout.Add(gradeLabel);
            absoluteLayout.SetLayoutBounds(gradeLabel, new Rect((App.screenWidth / 2) - (50 * App.screenHeightAdapter), 230 * App.screenHeightAdapter, 100 * App.screenHeightAdapter, 30 * App.screenHeightAdapter));

            return 1;
		}
		

		public ProfileCS()
		{
			Debug.WriteLine("ProfileCS");
			NavigationPage.SetBackButtonTitle(this, "");
			this.initLayout();
			this.initSpecificLayout();

		}

		public void CreateStackButtons() {
            var buttonWidth = (App.screenWidth - 15 * App.screenWidthAdapter) / 4;

            geralButton = new MenuButton("GERAL", buttonWidth, 60);
			geralButton.button.Clicked += OnGeralButtonClicked;
			identificacaoButton = new MenuButton("ID",buttonWidth, 60);
			identificacaoButton.button.Clicked += OnIdentificacaoButtonClicked;
			moradaButton = new MenuButton("CONTACTOS", buttonWidth, 60);
			moradaButton.button.Clicked += OnMoradaButtonClicked;
			encEducacaoButton = new MenuButton("FATURAÇÃO",buttonWidth, 60);
			encEducacaoButton.button.Clicked += OnEncEducacaoButtonClicked;

			stackButtons = new Microsoft.Maui.Controls.StackLayout
			{
                Spacing = 5,
                Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Children =
				{
					geralButton,
					identificacaoButton,
					moradaButton,
					encEducacaoButton
				}
			};

			absoluteLayout.Add(stackButtons);
            absoluteLayout.SetLayoutBounds(stackButtons, new Rect(0, 250 * App.screenHeightAdapter, App.screenWidth, 60 * App.screenHeightAdapter));

			geralButton.activate();
			identificacaoButton.deactivate();
			moradaButton.deactivate();
			encEducacaoButton.deactivate();
		}

		public void createChangePasswordButton()
		{
            Image changePasswordImage = new Image
            {
                Source = "iconpassword.png",
                Aspect = Aspect.AspectFit
            };

            TapGestureRecognizer changePasswordImage_tapEvent = new TapGestureRecognizer();
            changePasswordImage_tapEvent.Tapped += OnChangePasswordButtonClicked;
            changePasswordImage.GestureRecognizers.Add(changePasswordImage_tapEvent);

			absoluteLayout.Add(changePasswordImage);
            absoluteLayout.SetLayoutBounds(changePasswordImage, new Rect((App.screenWidth) - (47.5 * App.screenHeightAdapter), y_button_right * App.screenHeightAdapter, 35 * App.screenHeightAdapter, 35 * App.screenHeightAdapter));

            //Debug.Print("y_button_right 0 = " + y_button_right);

            Label changePasswordLabel = new Label
            {
                FontFamily = "futuracondensedmedium",
                Text = "Segurança",
                TextColor = App.normalTextColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start,
                FontSize = App.smallTextFontSize
            };

			absoluteLayout.Add(changePasswordLabel);
            absoluteLayout.SetLayoutBounds(changePasswordLabel, new Rect((App.screenWidth) - (60 * App.screenHeightAdapter), (y_button_right + 37) * App.screenHeightAdapter, 60 * App.screenHeightAdapter, 15 * App.screenHeightAdapter));

            y_button_right = y_button_right + 60;
        }


        public void createChangeMemberButton()
        {
            if (App.members.Count > 1)
            {

                Button changeMemberButton = new Button { HorizontalOptions = LayoutOptions.Center, BackgroundColor = Colors.Transparent, ImageSource = "botaoalmudarconta.png", HeightRequest = 30 };

                /*gridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = buttonWidth });
				//RoundButton changeMemberButton = new RoundButton("Login Outro Sócio", buttonWidth-5, 40);
				changeMemberButton.Clicked += OnChangeMemberButtonClicked;*/

                Image changeMemberImage = new Image
                {
                    Source = "iconescolherutilizador.png",
                    Aspect = Aspect.AspectFit
                };

                TapGestureRecognizer changeMemberImage_tapEvent = new TapGestureRecognizer();
                changeMemberImage_tapEvent.Tapped += OnChangeMemberButtonClicked;
                changeMemberImage.GestureRecognizers.Add(changeMemberImage_tapEvent);

				absoluteLayout.Add(changeMemberImage);
                absoluteLayout.SetLayoutBounds(changeMemberImage, new Rect((12.5 * App.screenHeightAdapter), y_button_left * App.screenHeightAdapter, 35 * App.screenHeightAdapter, 35 * App.screenHeightAdapter));

                Label changeMemberLabel = new Label
                {
                    FontFamily = "futuracondensedmedium",
                    Text = "Mudar Utilizador",
                    TextColor = App.normalTextColor,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Start,
                    FontSize = App.smallTextFontSize
                };

				absoluteLayout.Add(changeMemberLabel);
                absoluteLayout.SetLayoutBounds(changeMemberLabel, new Rect(0, (y_button_left + 37) * App.screenHeightAdapter, 60 * App.screenHeightAdapter, 15 * App.screenHeightAdapter));

                //Debug.Print("y_button_left 0 = " + y_button_left);

                y_button_left = y_button_left + 60;

                //Debug.Print("y_button_left 1 = " + y_button_left);
            }
        }

        public void createChangeStudentButton()
        {

			Debug.Print("createChangeStudentButton App.original_member.students_countt = " + App.original_member.students_count);
            if (App.original_member.students_count > 1)
            {

                Button changeStudentButton = new Button { HorizontalOptions = LayoutOptions.Center, BackgroundColor = Colors.Transparent, ImageSource = "botaoalmudarconta.png", HeightRequest = 30 };

                /*gridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = buttonWidth });
				//RoundButton changeMemberButton = new RoundButton("Login Outro Sócio", buttonWidth-5, 40);
				changeMemberButton.Clicked += OnChangeMemberButtonClicked;*/

                Image changeStudentImage = new Image
                {
                    Source = "iconescolheraluno.png",
                    Aspect = Aspect.AspectFit
                };

                TapGestureRecognizer changeStudentImage_tapEvent = new TapGestureRecognizer();
                changeStudentImage_tapEvent.Tapped += OnChangeStudentButtonClicked;
                changeStudentImage.GestureRecognizers.Add(changeStudentImage_tapEvent);

				absoluteLayout.Add(changeStudentImage);
                absoluteLayout.SetLayoutBounds(changeStudentImage, new Rect((12.5 * App.screenHeightAdapter), y_button_left * App.screenHeightAdapter, 35 * App.screenHeightAdapter, 35 * App.screenHeightAdapter));

                Label changeStudentLabel = new Label
                {
                    FontFamily = "futuracondensedmedium",
                    Text = "Alunos",
                    TextColor = App.normalTextColor,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Start,
                    FontSize = App.smallTextFontSize
                };

				absoluteLayout.Add(changeStudentLabel);
                absoluteLayout.SetLayoutBounds(changeStudentLabel, new Rect(0, (y_button_left + 37) * App.screenHeightAdapter, 60 * App.screenHeightAdapter, 15 * App.screenHeightAdapter));

                y_button_left = y_button_left + 60;
            }
        }

		public void createApproveStudentButton()
		{
            if ((App.member.isInstrutorResponsavel == "1") | (App.member.isResponsavelAdministrativo == "1"))
            {
                Image membersToApproveImage = new Image
                {
                    Source = "iconaprovarinscricoes.png",
                    Aspect = Aspect.AspectFit
                };

                TapGestureRecognizer membersToApproveImage_tapEvent = new TapGestureRecognizer();
                membersToApproveImage_tapEvent.Tapped += membersToApproveImage_Clicked;
                membersToApproveImage.GestureRecognizers.Add(membersToApproveImage_tapEvent);


				absoluteLayout.Add(membersToApproveImage);
                absoluteLayout.SetLayoutBounds(membersToApproveImage, new Rect((12.5 * App.screenHeightAdapter), y_button_left * App.screenHeightAdapter, 35 * App.screenHeightAdapter, 35 * App.screenHeightAdapter));

                Label membersToApproveLabel = new Label
                {
                    FontFamily = "futuracondensedmedium",
                    Text = "Aprovar Inscrições",
                    TextColor = App.normalTextColor,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Start,
                    FontSize = App.smallTextFontSize
                };

				absoluteLayout.Add(membersToApproveLabel);
                absoluteLayout.SetLayoutBounds(membersToApproveLabel, new Rect(0, (y_button_left + 37) * App.screenHeightAdapter, 60 * App.screenHeightAdapter, 15 * App.screenHeightAdapter));

                y_button_left = y_button_left + 60;
            }
        }

        public void createDocumentsNKSButton()
        {
            Image membersToApproveImage = new Image
            {
                Source = "iconaprovarinscricoes.png",
                Aspect = Aspect.AspectFit
            };

            TapGestureRecognizer membersToApproveImage_tapEvent = new TapGestureRecognizer();
            membersToApproveImage_tapEvent.Tapped += documentsImage_Clicked;
            membersToApproveImage.GestureRecognizers.Add(membersToApproveImage_tapEvent);


            absoluteLayout.Add(membersToApproveImage);
            absoluteLayout.SetLayoutBounds(membersToApproveImage, new Rect((12.5 * App.screenHeightAdapter), y_button_left * App.screenHeightAdapter, 35 * App.screenHeightAdapter, 35 * App.screenHeightAdapter));

            Label membersToApproveLabel = new Label
            {
                FontFamily = "futuracondensedmedium",
                Text = "Documentos NKS",
                TextColor = App.normalTextColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start,
                FontSize = App.smallTextFontSize
            };

            absoluteLayout.Add(membersToApproveLabel);
            absoluteLayout.SetLayoutBounds(membersToApproveLabel, new Rect(0, (y_button_left + 37) * App.screenHeightAdapter, 60 * App.screenHeightAdapter, 15 * App.screenHeightAdapter));

            y_button_left = y_button_left + 60;
        }

        public async void CreateGridButtons()
		{
			//createDocumentsNKSButton();

			
            
            createChangeMemberButton();
            createChangeStudentButton();
			//createApproveStudentButton();

            createChangePasswordButton();
            //CreateObjectivesButton();
			_ = await CreateQuotaButton();
            _ = await CreatePaymentsButton();
            _ = await CreateMedicalExamButton();
        }

		public void CreateGridGeral() {

			gridGeral = new Microsoft.Maui.Controls.Grid { Padding = 0, ColumnSpacing = 5 * App.screenWidthAdapter, HorizontalOptions = LayoutOptions.FillAndExpand, RowSpacing = 5 * App.screenWidthAdapter };
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            //gridGeral.RowDefinitions.Add(new RowDefinition { Height = 1 });
            gridGeral.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); //GridLength.Auto
			gridGeral.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 

			Label number_memberLabel = new FormLabel { Text = "Nº SÓCIO" };
			FormValue number_memberValue = new FormValue(App.member.number_member);

			FormLabel nameLabel = new FormLabel { Text = "NOME", HorizontalTextAlignment = TextAlignment.Start };
			nameValue = new FormValueEdit(App.member.name);

			FormLabel birthdateLabel = new FormLabel { Text = "NASCIMENTO"};
            //FormValue birthdateValue = new FormValueEditDate (member.birthdate?.ToString("yyyy-MM-dd"));
            //FormValue birthdateValue = new FormValue(App.member.birthdate);
            birthdateValue = new FormValueEditDate(App.member.birthdate);

            FormLabel registrationdateLabel = new FormLabel { Text = "INSCRIÇÃO"};
            //FormValueEditDate registrationdateValue = new FormValueEditDate(App.member.registrationdate?.ToString("yyyy-MM-dd"));
            FormValue registrationdateValue = new FormValue(App.member.registrationdate?.ToString("yyyy-MM-dd"));

            FormLabel addressLabel = new FormLabel { Text = "MORADA" };
            addressValue = new FormValueEdit(App.member.address);

            FormLabel cityLabel = new FormLabel { Text = "CIDADE" };
            cityValue = new FormValueEdit(App.member.city);

            FormLabel postalcodeLabel = new FormLabel { Text = "CÓDIGO POSTAL" };
            postalcodeValue = new FormValueEdit(App.member.postalcode);

            gridGeral.Add(number_memberLabel, 0, 0);
			gridGeral.Add(number_memberValue, 1, 0);

			gridGeral.Add(nameLabel, 0, 1);
			gridGeral.Add(nameValue, 1, 1);

			gridGeral.Add(birthdateLabel, 0, 2);
			gridGeral.Add(birthdateValue, 1, 2);

			gridGeral.Add(registrationdateLabel, 0, 3);
			gridGeral.Add(registrationdateValue, 1, 3);

            gridGeral.Add(addressLabel, 0, 4);
            gridGeral.Add(addressValue, 1, 4);

            gridGeral.Add(cityLabel, 0, 5);
            gridGeral.Add(cityValue, 1, 5);

            gridGeral.Add(postalcodeLabel, 0, 6);
            gridGeral.Add(postalcodeValue, 1, 6);

            /*absoluteLayout.Add(gridGeral,
				xConstraint: )0),
				yConstraint: )240),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 230; // center of image (which is 40 wide)
				})
			);*/
        }

		public void CreateGridIdentificacao()
		{

			gridIdentificacao = new Microsoft.Maui.Controls.Grid { Padding = 0, ColumnSpacing = 5 * App.screenWidthAdapter, RowSpacing = 5 * App.screenWidthAdapter };
			gridIdentificacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridIdentificacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridIdentificacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridIdentificacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridIdentificacao.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); //GridLength.Auto
			gridIdentificacao.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 

			FormLabel cc_numberLabel = new FormLabel { Text = "CC" };
            cc_numberValue = new FormValue(App.member.cc_number);

            FormLabel nifLabel = new FormLabel { Text = "NIF"};
			nifValue = new FormValue(App.member.nif);

			FormLabel fnkpLabel = new FormLabel { Text = "FNKP" };
			FormValue fnkpValue = new FormValue (App.member.number_fnkp);

            FormLabel awikpLabel = new FormLabel { Text = "AWIKP" };
            FormValue awikpValue = new FormValue(App.member.number_awikp);

            gridIdentificacao.Add(cc_numberLabel, 0, 0);
			gridIdentificacao.Add(cc_numberValue, 1, 0);

			gridIdentificacao.Add(nifLabel, 0, 1);
			gridIdentificacao.Add(nifValue, 1, 1);

			gridIdentificacao.Add(fnkpLabel, 0, 2);
			gridIdentificacao.Add(fnkpValue, 1, 2);

			gridIdentificacao.Add(awikpLabel, 0, 3);
            gridIdentificacao.Add(awikpValue, 1, 3);
            /*absoluteLayout.Add(gridIdentificacao,
				xConstraint: )0),
				yConstraint: )230),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 230; // center of image (which is 40 wide)
				})
			);*/
        }

		public void CreateGridMorada()
		{

			gridMorada = new Microsoft.Maui.Controls.Grid { Padding = 0, ColumnSpacing = 5 * App.screenWidthAdapter, RowSpacing = 5 * App.screenWidthAdapter };
			gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            gridMorada.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); //GridLength.Auto
			gridMorada.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 


			FormLabel emailLabel = new FormLabel { Text = "EMAIL" };
			emailValue = new FormValueEdit(App.member.email);

			FormLabel phoneLabel = new FormLabel { Text = "TELEFONE" };
			phoneValue = new FormValueEdit(App.member.phone);

            FormLabel EncEducacao1Label = new FormLabel { Text = "ENCARREGADO DE EDUCAÇÃO", FontSize = App.itemTitleFontSize };

            FormLabel EncEducacao1NomeLabel = new FormLabel { Text = "NOME" };
            EncEducacao1NomeValue = new FormValueEdit(App.member.name_enc1);

            FormLabel EncEducacao1PhoneLabel = new FormLabel { Text = "TELEFONE" };
            EncEducacao1PhoneValue = new FormValueEdit(App.member.phone_enc1);

            FormLabel EncEducacao1MailLabel = new FormLabel { Text = "MAIL" };
            EncEducacao1MailValue = new FormValueEdit(App.member.mail_enc1);


            gridMorada.Add(emailLabel, 0, 0);
			gridMorada.Add(emailValue, 1, 0);

			gridMorada.Add(phoneLabel, 0, 1);
			gridMorada.Add(phoneValue, 1, 1);

            gridMorada.Add(EncEducacao1Label, 0, 2);
            Microsoft.Maui.Controls.Grid.SetColumnSpan(EncEducacao1Label, 2);


            gridMorada.Add(EncEducacao1NomeLabel, 0, 3);
            gridMorada.Add(EncEducacao1NomeValue, 1, 3);

            gridMorada.Add(EncEducacao1PhoneLabel, 0, 4);
            gridMorada.Add(EncEducacao1PhoneValue, 1, 4);

            gridMorada.Add(EncEducacao1MailLabel, 0, 5);
            gridMorada.Add(EncEducacao1MailValue, 1, 5);


            /*absoluteLayout.Add(gridMorada,
				xConstraint: )0),
				yConstraint: )230),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 230; // center of image (which is 40 wide)
				})
			);*/
        }

        public void CreateGridFaturacao()
		{

			gridFaturacao = new Microsoft.Maui.Controls.Grid { Padding = 0, ColumnSpacing = 5 * App.screenWidthAdapter, RowSpacing = 5 * App.screenWidthAdapter };
            gridFaturacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridFaturacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridFaturacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridFaturacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridFaturacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridFaturacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridFaturacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridFaturacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridFaturacao.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            gridFaturacao.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

			FormLabel FaturaNomeLabel = new FormLabel { Text = "NOME" };
			FaturaNomeValue = new FormValueEdit(App.member.faturacao_nome);

			FormLabel FaturaMoradaLabel = new FormLabel { Text = "MORADA" };
			FaturaMoradaValue = new FormValueEdit(App.member.faturacao_morada);

			FormLabel FaturaCidadeLabel = new FormLabel { Text = "CIDADE" };
			FaturaCidadeValue = new FormValueEdit(App.member.faturacao_cidade);

            FormLabel CodPostalLabel = new FormLabel { Text = "COD POSTAL" };
            CodPostalValue = new FormValueEdit(App.member.faturacao_codpostal);

            FormLabel NIFLabel = new FormLabel { Text = "NIF" };
            FaturaNIFValue = new FormValueEdit(App.member.faturacao_nif);

            gridFaturacao.Add(FaturaNomeLabel, 0, 1);
            gridFaturacao.Add(FaturaNomeValue, 1, 1);

            gridFaturacao.Add(FaturaMoradaLabel, 0, 2);
            gridFaturacao.Add(FaturaMoradaValue, 1, 2);

            gridFaturacao.Add(FaturaCidadeLabel, 0, 3);
            gridFaturacao.Add(FaturaCidadeValue, 1, 3);


            gridFaturacao.Add(CodPostalLabel, 0, 4);
            gridFaturacao.Add(CodPostalValue, 1, 4);

            gridFaturacao.Add(NIFLabel, 0, 5);
            gridFaturacao.Add(FaturaNIFValue, 1, 5);

            /*absoluteLayout.Add(gridFaturacao,
				xConstraint: )0),
				yConstraint: )230),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 230; // center of image (which is 40 wide)
				})
			);*/
        }


		async void OnGeralButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnGeralButtonClicked");
			geralButton.activate();
			identificacaoButton.deactivate();
			moradaButton.deactivate();
			encEducacaoButton.deactivate();

			scrollView.Content = gridGeral;

			if (enteringPage == false) {
				await UpdateMemberInfo();
            }
            enteringPage = false;

        }

		async void OnIdentificacaoButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnIdentificacaoButtonClicked");
			geralButton.deactivate();
			identificacaoButton.activate();
			moradaButton.deactivate();
			encEducacaoButton.deactivate();

			scrollView.Content = gridIdentificacao;

			await UpdateMemberInfo();
		}


		async void OnMoradaButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnMoradaButtonClicked");

			geralButton.deactivate();
			identificacaoButton.deactivate();
			moradaButton.activate();
			encEducacaoButton.deactivate();

			scrollView.Content = gridMorada;

			await UpdateMemberInfo();
		}

		async void OnEncEducacaoButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnEncEducacaoButtonClicked");

			geralButton.deactivate();
			identificacaoButton.deactivate();
			moradaButton.deactivate();
			encEducacaoButton.activate();

            scrollView.Content = gridFaturacao;

			await UpdateMemberInfo();
		}

		async void OnLogoutButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnLogoutButtonClicked");

			Preferences.Default.Remove("EMAIL");
            Preferences.Default.Remove("PASSWORD");
            Preferences.Default.Remove("SELECTEDUSER");
			App.member = null;
			App.members = null;

			Application.Current.MainPage = new NavigationPage(new LoginPageCS(""))
			{
				BarBackgroundColor = App.backgroundColor,
				BarTextColor = App.normalTextColor//FromRgb(75, 75, 75)
			};
		}

		async void OnChangePasswordButtonClicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new ChangePasswordPageCS(App.member));
		}

		async void OnChangeMemberButtonClicked(object sender, EventArgs e)
		{
			changeMember = true;

			//Navigation.PushAsync(new SelectMemberPageCS());
			Navigation.InsertPageBefore(new SelectMemberPageCS(), this);
			await Navigation.PopAsync();
			await Navigation.PopAsync();
		}

		async void OnChangeStudentButtonClicked(object sender, EventArgs e)
		{
            changeMember = true;
			Navigation.InsertPageBefore(new SelectStudentPageCS(), this);
			await Navigation.PopAsync();
			await Navigation.PopAsync();

			//Navigation.PushAsync(new SelectStudentPageCS());
		}

		async void OnBackOriginalButtonClicked(object sender, EventArgs e)
		{
			changeMember = true;
			App.member = App.original_member;
            //Navigation.PushAsync(new MainTabbedPageCS(""));
            App.Current.MainPage = new NavigationPage(new MainTabbedPageCS("", ""))
            {
                BarBackgroundColor = App.backgroundColor,
                BarTextColor = App.normalTextColor//FromRgb(75, 75, 75)
            };

		}


        async void membersToApproveImage_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ApproveRegistrationPageCS());
        }

		async void documentsImage_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Browser.OpenAsync("https://karatesangalhos.pt/", BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
            }
        }

        

        async void OnQuotaButtonClicked(object sender, EventArgs e)
		{

			//activateButton.IsEnabled = false;

			MemberManager memberManager = new MemberManager();

			if (App.member.currentFee is null)
			{

                showActivityIndicator();
				var result_create = await memberManager.CreateFee(App.member.id, App.member.member_type, DateTime.Now.ToString("yyyy"));
                hideActivityIndicator();
                if (result_create == "-1")
				{
					Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
					{
						BarBackgroundColor = App.backgroundColor,
						BarTextColor = App.normalTextColor
					};
					return;
				}
				var result_get = await GetCurrentFees(App.member);
				if (result_get == -1)
				{
					Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
					{
						BarBackgroundColor = App.backgroundColor,
						BarTextColor = App.normalTextColor
					};
					return;
				}
                await Navigation.PushAsync(new QuotasPaymentPageCS(App.member));
            }
			else
			{
                await Navigation.PushAsync(new QuotasListPageCS());
			}

		}

        async void OnPaymentsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AllPaymentsPageCS());
        }

        async void OnMedicalExamButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MedicalExamPageCS());
        }


        async void OnObjectivesButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ObjectivesPageCS());   
        }


        async Task<int> GetCurrentFees(Model.Member member)
		{
			Debug.WriteLine("GetCurrentFees");
			MemberManager memberManager = new MemberManager();

			var result = await memberManager.GetCurrentFees(App.member);
			if (result == -1)
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = App.backgroundColor,
					BarTextColor = App.normalTextColor
				};
				return -1;
			}
			return result;
        }


        public async Task<string> ValidateMemberData()
        {
            if (string.IsNullOrEmpty(postalcodeValue.entry.Text))
            {
                postalcodeValue.entry.Text = "";
            }

            Debug.WriteLine("ValidateMemberData " + nameValue.entry.Text);
            if (nameValue.entry.Text == "")
            {
                nameValue.entry.Text = App.member.name;
                await DisplayAlert("DADOS INVÁLIDOS", "O nome introduzido não é válido.", "Ok");
                return "-1";
            }

            else if (phoneValue.entry.Text == null)
            {
                phoneValue.entry.Text = App.member.phone;
                await DisplayAlert("DADOS INVÁLIDOS", "Tem de introduzir o telefone.", "Ok");
                return "-1";
            }
            else if (!Regex.IsMatch(phoneValue.entry.Text, @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$"))
            {
                phoneValue.entry.Text = App.member.phone;
                await DisplayAlert("DADOS INVÁLIDOS", "O telefone introduzido não é válido.", "Ok");
                return "-1";
            }
            else if (!Regex.IsMatch((postalcodeValue.entry.Text), @"^\d{4}-\d{3}$"))
            {
                postalcodeValue.entry.Text = App.member.postalcode;
                await DisplayAlert("DADOS INVÁLIDOS", "O código postal introduzido não é válido.", "Ok");
                return "-1";
            }
            return "0";
        }
        

async Task<string> UpdateMemberInfo()
		{
			Debug.Print("AQUIIII UpdateMemberInfo");

			if (App.member != null)
			{
                ValidateMemberData();
				
				Debug.WriteLine("UpdateMemberInfo "+ App.member.name);
				App.member.name = nameValue.entry.Text;
				App.member.email = emailValue.entry.Text;
               // App.member.nif = nifValue.entry.Text;
                //App.member.cc_number = cc_numberValue.entry.Text;
                App.member.birthdate = birthdateValue.entry.Text;
                App.member.phone = phoneValue.entry.Text;
				App.member.address = addressValue.entry.Text;
				App.member.city = cityValue.entry.Text;
				App.member.postalcode = postalcodeValue.entry.Text;
				App.member.name_enc1 = EncEducacao1NomeValue.entry.Text;
				App.member.phone_enc1 = EncEducacao1PhoneValue.entry.Text;
				App.member.mail_enc1 = EncEducacao1MailValue.entry.Text;
                App.member.faturacao_nome = FaturaNomeValue.entry.Text;
                App.member.faturacao_morada = FaturaMoradaValue.entry.Text;
                App.member.faturacao_cidade = FaturaCidadeValue.entry.Text;
                App.member.faturacao_codpostal = CodPostalValue.entry.Text;
                App.member.faturacao_nif = FaturaNIFValue.entry.Text;

                //App.member.faturacao_nome = EncEducacao2MailValue.entry.Text;


                MemberManager memberManager = new MemberManager();

				var result = await memberManager.UpdateMemberInfo(App.member);
				if (result == "-1")
				{
					Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
					{
						BarBackgroundColor = App.backgroundColor,
						BarTextColor = App.normalTextColor
					};
					return "-1";
				}
				return result;
			}
			return "";
		}

        void memberPhotoImageTappedAsync(object sender, System.EventArgs e)
        {
            displayMemberPhotoImageActionSheet();
        }

        async Task<string> displayMemberPhotoImageActionSheet()
        {
            var actionSheet = await DisplayActionSheet("Fotografia Sócio", "Cancel", null, "Tirar Foto", "Galeria de Imagens");
            switch (actionSheet)
            {
                case "Cancel":
                    break;
                case "Tirar Foto":
                    TakeAPhotoTapped();
                    break;
                case "Galeria de Imagens":
                    OpenGalleryTapped();
                    break;
            }
            return "";
        }


        async void OpenGalleryTapped()
        {
            showActivityIndicator();

            ImageService imageService = new ImageService();
            var result = await imageService.PickPhotoAsync();

            
            if (result != null)
            {
                stream = await Constants.ResizePhotoStream(result); //result.OpenReadAsync();
                Stream localstream = await Constants.ResizePhotoStream(result);  //await result.OpenReadAsync();

                memberPhotoImage.Source = ImageSource.FromStream(() => localstream);
                MemberManager memberManager = new MemberManager();
                await memberManager.Upload_Member_Photo(stream);

            }
            hideActivityIndicator();
        }


        async void TakeAPhotoTapped()
        {
            showActivityIndicator();
            ImageService imageService = new ImageService();
            var result = await imageService.CapturePhotoAsync();

            
            if (result != null)
            {
                stream = await Constants.ResizePhotoStream(result); //result.OpenReadAsync();
                Stream localstream = await Constants.ResizePhotoStream(result);  //await result.OpenReadAsync();

                memberPhotoImage.Source = ImageSource.FromStream(() => localstream);

                MemberManager memberManager = new MemberManager();
                await memberManager.Upload_Member_Photo(stream);
            }

            hideActivityIndicator();

        }

    }
}