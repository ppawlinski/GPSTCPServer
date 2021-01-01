﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using GPSTCPClient.View;
using GPSTCPClient.Models;
using System.Text.Json;
using System.IO;

namespace GPSTCPClient
{
    public static class Client
    {
        private static readonly string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "log.txt");
        private static StreamWriter file = new StreamWriter(logFilePath);
        public static TcpClient TCP { get; set; }
        private static TcpListener listener { get; set; }
        private static string login;
        public static string ApiKey;
        public async static void Send(string message)
        {
            await TCP.GetStream().WriteAsync(Encoding.UTF8.GetBytes(message));
        }
        public async static Task Connect(string ipAddress, int port)
        {
            TCP = new TcpClient();
            if (TCP.Connected)
            {
                return;
            }
            try
            {
                await TCP.ConnectAsync(IPAddress.Parse(ipAddress), port);
                listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            }
            catch (FormatException)
            {
                throw new Exception("Błędny format adresu ip");
            }
            catch (Exception)
            {
                throw new Exception("Błąd przy próbie połączenia");
            }
        }
        public async static Task<bool> Login(string login_, string password)
        {

            byte[] buffer = new byte[1024];
            string result = "";
            string md5 = Md5Hasher.CreateMD5(password);
            Send($"LOGIN {login_} {md5}");
            result = await getUserInput(new byte[1024]);
            /*await Listener.AcceptTcpClientAsync().ContinueWith(async (listen) =>
            {
                
            });*/
            if (result == "SUCCESS")
            {
                login = login_;
                return true;
            }
            return false;
        }

        public async static Task<bool> Register(string login_, string password)
        {
            string md5 = Md5Hasher.CreateMD5(password);
            Send($"CREATEACCOUNT {login_} {md5}");
            string result = await getUserInput(new byte[1024]);
            if (result == "ACCOUNTCREATED")
            {
                //login = login_;
                return true;
            }
            else return false;
        }
        public async static Task Logout()
        {
            login = null;
            Send($"LOGOUT");
            //string result = "";
            //await Listener.AcceptTcpClientAsync().ContinueWith(async (listen) =>
            //{
            //    result = await getUserInput(new byte[1024]);
            //});
        }
        public static void Disconnect()
        {
            TCP.Close();
            TCP.Dispose();
            TCP = null;
            listener = null;
        }
        private async static Task<string> getUserInput(byte[] buffer)
        {
            await TCP.GetStream().ReadAsync(buffer, 0, buffer.Length);
            //await TCP.GetStream().ReadAsync(new byte[10]);
            return Encoding.UTF8.GetString(buffer).Trim().Replace("\0", String.Empty);
        }

        public static async Task<List<UserLocation>> GetMyAddresses()
        {
            Send($"LISTSAVEDADDRESSES");
            List<UserLocation> locations = new List<UserLocation>();
            string[] names = (await getUserInput(new byte[1024])).Split(' ');
            if (names[0] != "FAIL")
            {
                try
                {
                    foreach (var name in names)
                    {
                        if (name == "") continue;
                        Send($"GETSAVEDADDRESS {name}");
                        locations.Add(new UserLocation()
                        {
                            Name = name,
                            Address = JsonSerializer.Deserialize<Address[]>(await getUserInput(new byte[2048])).FirstOrDefault()
                        }
                        );
                    }
                }
                catch (Exception) { return locations; }
            }
            return locations;
        }

        public static async Task<Address[]> GetAddress(string location)
        {
            try
            {
                Send($"GETADDRESS {location.Replace(' ', '+')}");
                return JsonSerializer.Deserialize<Address[]>(await getUserInput(new byte[100000]));
            }
            catch (Exception)
            {
                return new Address[0];
            }
            
        }

        public static async Task<RouteModel[]> GetRoute(Address origin, Address destination)
        {
            Send($"GETROUTE {origin.Lon} {origin.Lat} {destination.Lon} {destination.Lat}");
            return JsonSerializer.Deserialize<RouteModel[]>(await getUserInput(new byte[100000]));
        }

        public static async Task<bool> AddAddress(Address address, string name)
        {
            Send($"ADDADDRESS {name} {address.OsmType} {address.OsmId}");
            return await getUserInput(new byte[1024]) == "SUCCESS";
        }

        public static async Task<bool> DeleteAddress(string name)
        {
            Send($"DELETEADDRESS {name}");
            return await getUserInput(new byte[1024]) == "SUCCESS";
        }

        public static async Task<bool> ChangePassword(string oldPassword, string newPassword)
        {
            oldPassword = Md5Hasher.CreateMD5(oldPassword);
            newPassword = Md5Hasher.CreateMD5(newPassword);
            Send($"CHANGEPASSWORD {oldPassword} {newPassword}");
            return await getUserInput(new byte[1024]) == "SUCCESS";
        }
    }
}
