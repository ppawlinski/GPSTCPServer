﻿using GPSTCPClient.ViewModel.Components;
using GPSTCPClient.ViewModel.MVVM;
using GPSTCPClient.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GPSTCPClient.ViewModel
{
    class RegisterVM : ViewModelBase
    {
        private bool returnEnabled;
        private bool registerEnabled;
        private string registerError;
        private string login;
        private string serverAddress;
        private string serverPort;
        private string errorColor;
        private MainVM mainVM;
        public RegisterVM(MainVM mainVM_)
        {
            mainVM = mainVM_;
            RegisterClickCommand = new Command(arg => RegisterAction(arg));
            ReturnClickCommand = new Command(arg => ReturnAction());
            ServerAddress = "127.0.0.1";
            ServerPort = "2048";
            Login = string.Empty;
            registerEnabled = true;
            returnEnabled = true;
            ErrorColor = "Red";
        }

        private void ReturnAction()
        {
            mainVM.NavigateTo("Login");
        }

        private async void RegisterAction(object arg)
        {
            ReturnEnabled = false;
            RegisterEnabled = false;
            mainVM.Loading = true;
            RegisterError = "";
            ErrorColor = "Red";
            if (((IEnumerable)arg).Cast<object>()
                            .Select(x => (PasswordBox)x)
                            .ToArray() is PasswordBox[] arr)
            {
                if (arr[0].Password != string.Empty && arr[1].Password != string.Empty && Login != string.Empty)
                {
                    if (arr[0].Password == arr[1].Password)
                    {
                        if (!await Client.Connect(serverAddress, int.Parse(ServerPort)))
                            RegisterError = "Błąd połączenia";
                        else if (await Client.Register(login, arr[0].Password))
                        {
                            RegisterError = "Pomyślnie utworzono konto.";
                            ErrorColor = "Green";
                            Login = string.Empty;
                        }
                        else
                            RegisterError = "Nie udało się utworzyć konta!";
                    }
                    else
                        RegisterError = "Hasła muszą być takie same!";
                }
                else
                    RegisterError = "Wszystkie pola muszą być wypełnione!";
            }
            ReturnEnabled = true;
            RegisterEnabled = true;
            mainVM.Loading = false;
        }
        public ICommand RegisterClickCommand { get; set; }
        public ICommand ReturnClickCommand { get; }
        public string ServerAddress
        {
            get
            {
                return serverAddress;
            }
            set
            {
                serverAddress = value;
                OnPropertyChanged(nameof(ServerAddress));
            }
        }
        public string ServerPort
        {
            get
            {
                return serverPort;
            }
            set
            {
                serverPort = value;
                OnPropertyChanged(nameof(ServerPort));
            }
        }
        public bool ReturnEnabled
        {
            get { return returnEnabled; }
            set
            {
                returnEnabled = value;
                OnPropertyChanged(nameof(ReturnEnabled));
            }
        }
        public bool RegisterEnabled
        {
            get { return registerEnabled; }
            set
            {
                registerEnabled = value;
                OnPropertyChanged(nameof(RegisterEnabled));
            }
        }
        public string RegisterError
        {
            get { return registerError; }
            set
            {
                registerError = value;
                OnPropertyChanged(nameof(RegisterError));
            }
        }
        public string Login
        {
            get { return login; }
            set
            {
                login = value;
                OnPropertyChanged(nameof(Login));
            }
        }
        public string ErrorColor
        {
            get { return errorColor; }
            set
            {
                errorColor = value;
                OnPropertyChanged(nameof(ErrorColor));
            }
        }
    }
}