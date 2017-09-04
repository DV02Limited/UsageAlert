using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;

namespace UsageAlert
{
    
    public class EmailGeneration
    {
        public static string CustomerEmail { get; set; }
        public int Result { get; }

        public EmailGeneration([In, MarshalAs(UnmanagedType.LPArray, SizeConst = 7)]string[] args)
        {
            Result = Gen(args);
        }

        private int Gen(string[] args)
        {

            try
            {
                //Set customer enail address defined on contents of CKeyResult
                switch (args[4])
                {
                    case "DV02":
                        CustomerEmail = "hdcalls@dv02.co.uk";
                        break;
                    case "Grunenthal":
                        CustomerEmail = "uk.ithelpdesk@grunenthal.com";
                        break;
                    case "Cleanbrite":
                        CustomerEmail = "customercare@cleanbrite.co.uk";
                        break;
                    case "Nordea Bank":
                        CustomerEmail = "dlldlondonit@nordea.com";
                        break;
                    case "DWA Claims Ltd":
                        CustomerEmail = "ashley@dwaclaims.co.uk";
                        break;
                    default:
                        CustomerEmail = null;
                        break;
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"Customer name {args[4]} does not match");
                Console.WriteLine(e.ToString());
                return 1;
            }

            string emailBody = File.ReadAllText(args[6]); //Read email body text and place in variable emailBody

            MailDefinition md = new MailDefinition()
            {
                From = "noreply@dv02.co.uk",
                CC = "hdcalls@dv02.co.uk",
                IsBodyHtml = true,
                Subject = args[5]
            };

            //Defines variables in email body and what arguments they will be replaced with
            ListDictionary replacements = new ListDictionary
                 {
                    { "{name}", args[0] },
                    { "{mobile}", args[1] },
                    { "{date}", args[2] },
                    { "{Amount_Exceeded}", args[3] }
                 };

            // Creates email message in variable msg
            CreateEmail(CustomerEmail, emailBody, md, replacements);
            return 0;
        }

        private static void CreateEmail(string CustomerEmail, string emailBody, MailDefinition md, ListDictionary replacements)
        {
            MailMessage msg = md.CreateMailMessage(CustomerEmail, replacements, emailBody, new System.Web.UI.Control());

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
        }
    }
}
