using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TSFramework.Core.Members.Mail;

namespace TSFramework.Core.Providers
{
    public static class SmtpClientFactory
    {
        public static SmtpClient CreateClient(ConfigMailModel config)
        {
            return new SmtpClient
            {
                Host = config.Host,
                Port = config.Port,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential
                    (config.UserCredential, config.UPass),
                EnableSsl = true
            };
        }
    }

    public class MailProvider
    {
        private static ConfigMailModel _config;

        private static MailProvider _instance;

        /// <summary>
        ///     Contructor mail provider
        /// </summary>
        /// <param name="config">Config model</param>
        private MailProvider(ConfigMailModel config)
        {
            _config = new ConfigMailModel
            {
                Port = config.Port,
                Host = config.Host,
                UPass = config.UPass,
                UserCredential = config.UserCredential,
                UserCredentialName = config.UserCredentialName
            };
        }

        /// <summary>
        ///     Create new Instance of MailProvider
        /// </summary>
        /// <returns></returns>
        public static MailProvider Instance()
        {
            _config = new ConfigMailModel
            {
                Host = ConfigurationManager.AppSettings["Email_Host"],
                Port = string.IsNullOrEmpty(ConfigurationManager.AppSettings["Email_Port"])
                    ? 587
                    : int.Parse(ConfigurationManager.AppSettings["Email_Port"]),
                UPass = ConfigurationManager.AppSettings["Email_UPass"],
                UserCredential = ConfigurationManager.AppSettings["Email_UserCredential"],
                UserCredentialName = ConfigurationManager.AppSettings["Email_UserCredentialName"]
            };
            if (_instance != null) return _instance;
            _instance = new MailProvider(_config);

            return _instance;
        }

        /// <summary>
        ///     Create new Instance of MailProvider
        /// </summary>
        /// <param name="sPort">Port mail server</param>
        /// <param name="sHost">Host mail server</param>
        /// <param name="sUserCredential">Mail address</param>
        /// <param name="sUPass">Password of mail</param>
        /// <param name="sUserCredentialName">Name of mail to display </param>
        /// <returns>MailProvider</returns>
        public static MailProvider Instance(int sPort, string sHost, string sUserCredential, string sUPass,
            string sUserCredentialName)
        {
            _config = new ConfigMailModel
            {
                Port = sPort,
                Host = sHost,
                UPass = sUPass,
                UserCredential = sUserCredential,
                UserCredentialName = sUserCredentialName
            };

            _instance = new MailProvider(_config);

            return _instance;
        }

        /// <summary>
        ///     Send email via task
        /// </summary>
        /// <param name="lstMails"></param>
        public void PushEmail(List<MailModel> lstMails)
        {
            var queueTasks = new Queue<Task>();
            lstMails.ForEach(mail => { Task.Factory.StartNew(() => { SendMail(mail); }); });
            new Thread(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        while (queueTasks.Count > 0)
                        {
                            var taskMail = queueTasks.Dequeue();
                            taskMail.Start();
                            while (!taskMail.IsCompleted)
                            {
                            }
                        }
                    }
                    catch (InvalidOperationException)
                    {
                    }
                });
            }).Start();
        }

        private bool IsOnline()
        {
            try
            {
                var tcp = new TcpClient();
                tcp.Connect(_config.Host, _config.Port);
                return true;
            }
            catch (Exception e)
            {
                new LogProvider().Error(e);
            }

            return false;
        }

        private void SendMail(MailModel mail)
        {
            if (!IsOnline()) return;

            var pm = new PreMailer.Net.PreMailer(mail.Body);
            var mailBody = pm.MoveCssInline(true);

            var nMail = new MailMessage
            {
                From = new MailAddress(mail.From ?? _config.UserCredential,
                    mail.DisplayNameFrom ?? _config.UserCredentialName),
                //BodyEncoding = Encoding.Unicode,
                //Body = mailBody.Html,
                //Body =  mail.Body,
                Subject = mail.Subject,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure,
                IsBodyHtml = mail.IsBodyHtml
            };

            if (mail.DicImgs != null && mail.DicImgs.Count > 0 && mail.IsBodyHtml)
            {
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                    (mailBody.Html, null, MediaTypeNames.Text.Html);

                foreach (var imgKey in mail.DicImgs.Keys)
                {
                    MemoryStream ms = new MemoryStream(mail.DicImgs[imgKey]);
                    LinkedResource inline = new LinkedResource(ms, MediaTypeNames.Image.Jpeg)
                    {
                        ContentId = imgKey
                    };
                    avHtml.LinkedResources.Add(inline);
                }
                nMail.AlternateViews.Add(avHtml);
            }
            if (mail.To != null && mail.To.Count > 0) mail.To.ForEach(m => { nMail.To.Add(new MailAddress(m)); });

            if (mail.Bcc != null && mail.Bcc.Count > 0) mail.Bcc.ForEach(m => { nMail.Bcc.Add(new MailAddress(m)); });

            if (mail.Cc != null && mail.Cc.Count > 0) mail.Cc.ForEach(m => { nMail.CC.Add(new MailAddress(m)); });

            var smtpClient = SmtpClientFactory.CreateClient(_config);
            smtpClient.Send(nMail);
        }
    }
}