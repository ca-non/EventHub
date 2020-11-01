using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
//ModelState validation flag
namespace UserLayer
{
    public class UserBusinessLayer
    {
        public bool addNewUser(User newUser, ModelStateDictionary mState)
        {
            bool modelStateFlag = true;
            // Username server validation
            if (newUser.UserName == null)
            {
                mState.AddModelError("UserName", "Username is required");
                modelStateFlag = false;
            }
            else if(newUser.UserName.Length > 200)
            {
                mState.AddModelError("UserName", "Username is too long");
                modelStateFlag = false;
            }
            else if(regualrexpression)
            {

            }

            // Password server validation
            if (newUser.Passwd == null || newUser.Passwd.Length > 2 || newUser.Passwd.Length == 0)
            {
                if(newUser.Passwd == null)
                {
                    mState.AddModelError("Passwd", "Password is required");
                    modelStateFlag = false;
                }
                else
                {
                    mState.AddModelError("passwd", "Password is too long");
                    modelStateFlag =  false;
                } 
            }

            string connectionString = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spAddUser", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@UserName";
                paramName.Value = newUser.UserName;
                cmd.Parameters.Add(paramName);

                SqlParameter paramEmail = new SqlParameter();
                paramEmail.ParameterName = "@Email";
                paramEmail.Value = newUser.Email;
                cmd.Parameters.Add(paramEmail);

                SqlParameter paramPassword = new SqlParameter();
                paramPassword.ParameterName = "@Passwd";
                paramPassword.Value = newUser.Passwd;
                cmd.Parameters.Add(paramPassword);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return modelStateFlag;
        }
    }
}
