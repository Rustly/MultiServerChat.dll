﻿using System;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Rests;
using TShockAPI;

namespace MultiServerChat
{
    public static class RestHelper
    {
        public static void SendChatMessage(TSPlayer ply, string formatted_text)
        {
            ThreadPool.QueueUserWorkItem(f =>
            {
                bool failure = false;
                var message = new Message()
                {
                    Text = String.Format(MultiServerChat.Config.ChatFormat,
                                            TShock.Config.ServerName,
                                            formatted_text),
                    Red = ply.Group.R,
                    Green = ply.Group.G,
                    Blue = ply.Group.B
                };

                var bytes = Encoding.UTF8.GetBytes(message.ToString());
                var base64 = Convert.ToBase64String(bytes);              

                foreach (var url in MultiServerChat.Config.RestURLs)
                {
                    var uri = String.Format("{0}/jl?token={1}", url, MultiServerChat.Config.Token);
                   
                    try
                    {
                        using (var wb = new WebClient())
                        {
                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            var data = new NameValueCollection();
                            var response = wb.UploadString(uri, "POST", base64);
                        }
                        
                        failure = false;
                    }
                    catch (Exception)
                    {
                        if (!failure)
                        {
                            TShock.Log.Error("Failed to make request to other server, server is down?");
                            failure = true;
                        }
                    }
                }
            });
        }

        public static void SendJoinMessage(TSPlayer ply)
        {
            ThreadPool.QueueUserWorkItem(f =>
            {
                bool failure = false;

                string text;

                if (TShock.Config.EnableGeoIP && TShock.Geo != null)
                    text = String.Format(MultiServerChat.Config.GeoJoinFormat, TShock.Config.ServerName, ply.Name);
                else
                    text = string.Format(MultiServerChat.Config.JoinFormat, TShock.Config.ServerName, ply.Name, ply.Country);

                var message = new Message()
                {
                    Text = text,
                    Red = Color.Yellow.R,
                    Green = Color.Yellow.G,
                    Blue = Color.Yellow.B
                };

                var bytes = Encoding.UTF8.GetBytes(message.ToString());
                var base64 = Convert.ToBase64String(bytes);

                foreach (var url in MultiServerChat.Config.RestURLs)
                {
                    var uri = String.Format("{0}/jl?token={1}", url, MultiServerChat.Config.Token);

                    try
                    {
                        using (var wb = new WebClient())
                        {
                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            var data = new NameValueCollection();
                            var response = wb.UploadString(uri, "POST", base64);
                        }
                        failure = false;
                    }
                    catch (Exception)
                    {
                        if (!failure)
                        {
                            TShock.Log.Error("Failed to make request to other server, server is down?");
                            failure = true;
                        }
                    }
                }
            });
        }

        public static void SendLeaveMessage(TSPlayer ply)
        {
            ThreadPool.QueueUserWorkItem(f =>
            {
                bool failure = false;
                var message = new Message()
                {
                    Text =
                        String.Format(MultiServerChat.Config.LeaveFormat, TShock.Config.ServerName, ply.Name),
                    Red = Color.Yellow.R,
                    Green = Color.Yellow.G,
                    Blue = Color.Yellow.B
                };

                var bytes = Encoding.UTF8.GetBytes(message.ToString());
                var base64 = Convert.ToBase64String(bytes);
                foreach (var url in MultiServerChat.Config.RestURLs)
                {
                    var uri = String.Format("{0}/jl?token={1}", url, MultiServerChat.Config.Token);

                    try
                    {
                        using (var wb = new WebClient())
                        {
                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            var data = new NameValueCollection();
                            var response = wb.UploadString(uri, "POST", base64);
                        }
                        failure = false;
                    }
                    catch (Exception)
                    {
                        if (!failure)
                        {
                            TShock.Log.Error("Failed to make request to other server, server is down?");
                            failure = true;
                        }
                    }
                }
            });
        }

        public static void RecievedMessage(RestRequestArgs args)
        {          
            if (args.Request.Method == HttpServer.Method.Post &&
                args.Request.ContentType.HeaderValue == "application/json")
            {
                using (StreamReader reader = new StreamReader(args.Request.Body))
                {
                    string data = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(data))
                    {
                        try
                        {
                            var bytes = Convert.FromBase64String(data);
                            var str = Encoding.UTF8.GetString(bytes);
                            var message = Message.FromJson(str);
                            //TShock.Utils.Broadcast(message.Text, message.Red, message.Green, message.Blue);
                            TSPlayer.All.SendMessage(message.Text, message.Red, message.Green, message.Blue);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }
    }
}
