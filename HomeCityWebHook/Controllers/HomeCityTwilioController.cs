using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio.AspNet.Common;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;
using System.Data.SqlClient;
using System.ComponentModel;

namespace HomeCityTwilioWebHook.Controllers
{
    public class HomeCityTwilioController : TwilioController
    {
        [HttpPost]
        public TwiMLResult Index(SmsRequest request)
        {

            var result = new List<string>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(request))
            {
                result.Add(property.Name + "=" + property.GetValue(request));
            }

            string newResult = string.Join("&", result);

            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["HomeCityDatabase"].ConnectionString;

            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "usp_tw_message_manage";
            //add any parameters the stored procedure might require
            cmd.Parameters.Add(new SqlParameter("@action", "insert"));
            cmd.Parameters.Add(new SqlParameter("@response", newResult));

            cnn.Open();

            object o = cmd.ExecuteScalar();

            cnn.Close();


            var response = new Twilio.TwiML.MessagingResponse();
            //response.Message("This is my first message");
            
            return TwiML(response);
        }

        
    }
}