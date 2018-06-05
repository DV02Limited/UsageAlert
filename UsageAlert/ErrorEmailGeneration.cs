using System;
using System.Net.Mail;

namespace UsageAlert
{
    public class ErrorEmailGeneration
    {
        public int Result { get; }

        public ErrorEmailGeneration(string emailBody, string p)
        {
            Result = CreateErrorEmail(emailBody, p);
        }

        private int CreateErrorEmail(string emailBody, string p)
        {
            try
            {
                MailMessage msg = new MailMessage(from: "donotreply@dv02.co.uk", to: "hdcalls@dv02.co.uk")
                {
                    Subject = "Mobile Usage Alert Discrepancy",
                    Body = emailBody,
                    IsBodyHtml = true
                };

                //Define SMTP Connection
                SmtpClient client = new SmtpClient()
                {
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    //UseDefaultCredentials = true,
                    Credentials = new System.Net.NetworkCredential("solidus1@dv02.co.uk", p),
                    Host = "smtp.office365.com"
                };

                // Send email
                client.Send(msg);
                msg.Dispose();
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
