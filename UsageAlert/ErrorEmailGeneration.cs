using System;
using System.Net.Mail;

namespace UsageAlert
{
    public class ErrorEmailGeneration
    {
        public int Result { get; }

        public ErrorEmailGeneration(string emailBody)
        {
            Result = CreateErrorEmail(emailBody);
        }

        private int CreateErrorEmail(string emailBody)
        {
            try
            {
                MailMessage msg = new MailMessage("noreply@dv02.co.uk", "hdcalls@dv02.co.uk")
                {
                    Subject = "Mobile Usage Alert Discrepancy",
                    Body = emailBody,
                    IsBodyHtml = true
                };

                //Define SMTP Connection
                SmtpClient client = new SmtpClient()
                {
                    Port = 25,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "dv02-co-uk.mail.protection.outlook.com"
                };

                // Send email
                client.Send(msg);
                return 0;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"Error sending email");
                Console.WriteLine(e.ToString());
                return 1;
            }

        }
    }
}
