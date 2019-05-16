using Teleboard.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Teleboard.Service
{
    public class MailService
    {
        public void Send(string tos, string ccs, string bccs, string subject, string body, string attachmentFiles)
        {
            Task.Run(() =>
            {
                try
                {

                    var bodyHtml = body;

                    MailMessage message = new MailMessage();

                    if (!string.IsNullOrEmpty(attachmentFiles))
                    {
                        string[] attachments = attachmentFiles.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var attachment in attachments)
                        {
                            if (File.Exists(attachment))
                            {
                                message.Attachments.Add(new Attachment(attachment));
                            }
                        }

                    }

                    message.From = new MailAddress(SettingsHelper.SmtpEmail);
                    message.To.Add(tos.Replace(";", ","));

                    if (!string.IsNullOrEmpty(ccs))
                    {
                        message.CC.Add(ccs.Replace(";", ","));
                    }

                    if (!string.IsNullOrEmpty(bccs))
                    {
                        message.Bcc.Add(bccs.Replace(";", ","));
                    }


                    message.Subject = subject;
                    message.IsBodyHtml = true;

                    message.Body = bodyHtml;

                    SmtpClient smtp = new SmtpClient(SettingsHelper.SmtpHost, SettingsHelper.SmtpPort);
                    smtp.Timeout = SettingsHelper.SmtpTimeout;
                    smtp.UseDefaultCredentials = SettingsHelper.SmtpUseDefaultCredential;
                    smtp.Credentials = new System.Net.NetworkCredential(SettingsHelper.SmtpEmail, SettingsHelper.SmtpPassword);
                    smtp.EnableSsl = SettingsHelper.SmtpEnableSsl;


                    smtp.Send(message);
                }
                catch(Exception)
                {
                    throw;
                }

            });
        }

    }
}