using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace EventHubMVC.Models
{
    public class MainAppLogic
    {
        public void addFeedback(ContactVM contactVM)
        {
            string cs = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spAddFeedback", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Email", contactVM.Email);
                cmd.Parameters.AddWithValue("@Subject", contactVM.Subject);
                cmd.Parameters.AddWithValue("@Message", contactVM.Message);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}