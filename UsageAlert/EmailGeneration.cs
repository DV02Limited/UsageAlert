using System;
using System.Data.SqlClient;
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

        public EmailGeneration([In, MarshalAs(UnmanagedType.LPArray, SizeConst = 8)]string[] args)
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
                        CustomerEmail = GetEmailAlertAddress(args[4],args[8]);
                        break;
                    case "Grunenthal":
                        CustomerEmail = GetEmailAlertAddress(args[4], args[8]);
                        break;
                    case "Cleanbrite":
                        CustomerEmail = GetEmailAlertAddress(args[4], args[8]);
                        break;
                    case "Nordea Bank":
                        GetEmailAlertExceptionAddress(CustomerEmail, args[8]);
                        if (CustomerEmail == "")
                        {
                            CustomerEmail = GetEmailAlertAddress(args[4], args[8]);
                        }
                        break;
                    case "DWA Claims Ltd":
                        CustomerEmail = GetEmailAlertAddress(args[4], args[8]);
                        break;
                    case "Brickendon Grange Golf Club":
                        CustomerEmail = GetEmailAlertAddress(args[4], args[8]);
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
                From = "donotreply@dv02.co.uk",
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
            CreateEmail(CustomerEmail, emailBody, md, replacements, args[7]);
            return 0;
        }

        private static void CreateEmail(string CustomerEmail, string emailBody, MailDefinition md, ListDictionary replacements, string p)
        {
            MailMessage msg = md.CreateMailMessage(CustomerEmail, replacements, emailBody, new System.Web.UI.Control());

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
        }

        private static string GetEmailAlertExceptionAddress(string mobile, string connectionString)
        {
            string result = "";

            using (SqlConnection conn = new SqlConnection())
            {

                string number = mobile;
                conn.ConnectionString = connectionString;
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT email from dbo.alert_exception where mobile = @0", conn);
                command.Parameters.Add(new SqlParameter("0", number));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = Convert.ToString(reader[0]);
                    }
                }
            }

            return result;
        }

        public static string GetEmailAlertAddress(string customerName, string connectionString)
        {
            string result = "";

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connectionString;
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT Customer_Email from dbo.Customers where Customer_Name = @0", conn);
                command.Parameters.Add(new SqlParameter("0", customerName));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = Convert.ToString(reader[0]);
                    }
                }
            }

            return result;
        }
    }
}
