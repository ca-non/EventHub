﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using UserBusinessLayer.ViewModels;
using System.Net.Mail;

namespace UserBusinessLayer
{
    public class UserBusinessLogic
    {
        public bool addNewUser(RegisterViewModel newUser, ModelStateDictionary mState)
        {
            bool modelStateFlag = true;


            // Username server validation
            if (String.IsNullOrEmpty(newUser.UserName))
            {
                mState.AddModelError("UserName", "Username is required");
                modelStateFlag = false;
            }
            else if (newUser.UserName.Length > 200)
            {
                mState.AddModelError("UserName", "Username is too long");
                modelStateFlag = false;
            }
            else
            {
                Regex pattern = new Regex(@"^\w+$");
                if (!pattern.IsMatch(newUser.UserName))
                {
                    mState.AddModelError("UserName", "Username can contain only letters, numbers and _ ");
                    modelStateFlag = false;
                }
                else if(checkUsernameRegister(newUser.UserName))
                {
                    mState.AddModelError("UserName", "Username already taken");
                    modelStateFlag = false;
                }
            }

            // Password server validation
            if (String.IsNullOrEmpty(newUser.Passwd) || newUser.Passwd.Length > 200 || newUser.Passwd.Length < 6)
            {
                if (String.IsNullOrEmpty(newUser.Passwd))
                {
                    mState.AddModelError("Passwd", "Password is required");
                    modelStateFlag = false;
                }
                else if (newUser.Passwd.Length > 200)
                {
                    mState.AddModelError("passwd", "Password is too long");
                    modelStateFlag = false;
                }
                else
                {
                    mState.AddModelError("passwd", "Password should be atleast 6 chracters long");
                    modelStateFlag = false;
                }
            }
            else
            {
                Regex pattern = new Regex(@"^[\w@$#*!]+$");
                if (!pattern.IsMatch(newUser.Passwd))
                {
                    mState.AddModelError("Passwd", "Invalid character entered for password");
                    modelStateFlag = false;
                }
            }

            // Confirm password server validation
            if (!String.IsNullOrEmpty(newUser.Passwd) && newUser.Passwd != newUser.ConfirmPasswd)
            {
                mState.AddModelError("ConfirmPasswd", "Passwords don't match");
                modelStateFlag = false;
            }

            // Email server validation
            if (String.IsNullOrEmpty(newUser.Email))
            {
                mState.AddModelError("Email", "Email is required");
                modelStateFlag = false;
            }
            else
            {
                Regex pattern = new Regex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$");
                if (!pattern.IsMatch(newUser.Email))
                {
                    mState.AddModelError("Email", "Invalid email entered");
                    modelStateFlag = false;
                }
                else if (checkUsernameRegister(newUser.Email))
                {
                    mState.AddModelError("Email", "Email already taken");
                    modelStateFlag = false;
                }
            }


            // Confirm email server validation
            if (!String.IsNullOrEmpty(newUser.Email) && newUser.Email != newUser.ConfirmEmail)
            {
                mState.AddModelError("ConfirmEmail", "Emails don't match 4");
                modelStateFlag = false;
            }

            if (modelStateFlag)
            {
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
            }


            return modelStateFlag;
        }

        public bool loginUser(LoginViewModel currentUser, ModelStateDictionary mState)
        {
            bool modelStateFlag = true;

            if (String.IsNullOrEmpty(currentUser.UsernameEmail))
            {
                mState.AddModelError("UsernameEmail", "Please enter username or email");
                modelStateFlag = false;
            }


            if (String.IsNullOrEmpty(currentUser.Passwd))
            {
                mState.AddModelError("Passwd", "Password cannot be empty");
                modelStateFlag = false;
            }

            if (modelStateFlag)
            {
                string Email = currentUser.UsernameEmail;
                string Username = currentUser.UsernameEmail;
                string connectionString = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;
                int count = 0;

                if (currentUser.UsernameEmail.Contains("@"))
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlCommand cmd = new SqlCommand("spCheckUserForEmail", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Email", Email);

                        SqlParameter outParameter = new SqlParameter();
                        outParameter.ParameterName = "@UserCount";
                        outParameter.SqlDbType = SqlDbType.Int;
                        outParameter.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outParameter);

                        con.Open();
                        cmd.ExecuteScalar();

                        count = (int)outParameter.Value;
                    }

                    if (count == 1)
                    {
                        // Check for Email & password
                        count = 0;
                        using (SqlConnection con = new SqlConnection(connectionString))
                        {
                            SqlCommand cmd = new SqlCommand("spLoginEmailPassword", con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@Email", Email);
                            cmd.Parameters.AddWithValue("@Passwd", currentUser.Passwd);

                            SqlParameter outParameter = new SqlParameter();
                            outParameter.ParameterName = "@UserCount";
                            outParameter.SqlDbType = SqlDbType.Int;
                            outParameter.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(outParameter);

                            con.Open();
                            cmd.ExecuteScalar();

                            count = (int)outParameter.Value;
                        }

                        if (count != 1)
                        {
                            modelStateFlag = false;
                            mState.AddModelError("LoginError", " Entered username or password is incorrect");
                        }
                    }
                    else
                    {
                        modelStateFlag = false;
                        mState.AddModelError("LoginError", " Entered username or password is incorrect");
                    }
                }
                else
                {
                    count = 0;
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlCommand cmd = new SqlCommand("spCheckUserForUsername", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Username", Username);

                        SqlParameter outParameter = new SqlParameter();
                        outParameter.ParameterName = "@UserCount";
                        outParameter.SqlDbType = SqlDbType.Int;
                        outParameter.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outParameter);

                        con.Open();
                        cmd.ExecuteScalar();

                        count = (int)outParameter.Value;
                    }

                    if (count == 1)
                    {
                        // Check for Username & password
                        count = 0;
                        using (SqlConnection con = new SqlConnection(connectionString))
                        {
                            SqlCommand cmd = new SqlCommand("spLoginUsernamePassword", con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@Username", Username);
                            cmd.Parameters.AddWithValue("@Passwd", currentUser.Passwd);

                            SqlParameter outParameter = new SqlParameter();
                            outParameter.ParameterName = "@UserCount";
                            outParameter.SqlDbType = SqlDbType.Int;
                            outParameter.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(outParameter);

                            con.Open();
                            cmd.ExecuteScalar();

                            count = (int)outParameter.Value;
                        }

                        if (count != 1)
                        {
                            modelStateFlag = false;
                            mState.AddModelError("LoginValidator", " Entered username or password is incorrect");
                        }
                    }
                    else
                    {
                        modelStateFlag = false;
                        mState.AddModelError("LoginValidator", " Entered username or password is incorrect");
                    }
                }

            }


            return modelStateFlag;
        }

        public bool checkUsernameRegister(string Username)
        {
            bool taken = false;

            string cs = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;
            int count;

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spCheckUsernameRegister", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", Username);

                SqlParameter outParameter = new SqlParameter();
                outParameter.ParameterName = "@Count";
                outParameter.SqlDbType = SqlDbType.Int;
                outParameter.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outParameter);

                con.Open();
                cmd.ExecuteScalar();

                count = (int)outParameter.Value;
            }

            if (count > 0)
            {
                taken = true;
                return taken;
            }
            else
            {
                return taken;
            }
        }

        public bool checkEmailRegister(string Email)
        {
            bool taken = false;

            string cs = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;
            int count;

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spCheckEmailRegister", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", Email);

                SqlParameter outParameter = new SqlParameter();
                outParameter.ParameterName = "@Count";
                outParameter.SqlDbType = SqlDbType.Int;
                outParameter.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outParameter);

                con.Open();
                cmd.ExecuteScalar();

                count = (int)outParameter.Value;
            }

            if (count > 0)
            {
                taken = true;
                return taken;
            }
            else
            {
                return taken;
            }
        }

        public bool resetPassword(ForgotPasswordViewModel forgotPasswordData)
        {
            string cs = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;
            bool returnFlag = false;

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spResetPassword", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", forgotPasswordData.Email);
                con.Open();

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while(rdr.Read())
                    {
                        if(Convert.ToBoolean(rdr["ReturnCode"]))
                        {
                            UserUtility userUtility = new UserUtility();
                            userUtility.SendPasswordResetEmail(forgotPasswordData.Email, rdr["Username"].ToString(), rdr["UniqueId"].ToString());
                            returnFlag = true;
                        }
                    }
                }
            }

            return returnFlag;
        }

        public bool checkForResetLinkValid(string uid)
        {
            if(uid == null)
            {
                return false;
            }

            bool flag = true;

            string cs = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spIsPasswordResetLinkValid", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@GUID", uid);

                con.Open();

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (Convert.ToBoolean(rdr["IsValidPasswordLink"]) == false)
                        {
                            flag = false;
                        }
                        else
                        {
                            using (SqlConnection con1 = new SqlConnection(cs))
                            {
                                SqlCommand cmd1 = new SqlCommand("spGetResetLinkTime", con1);
                                cmd1.CommandType = CommandType.StoredProcedure;
                                cmd1.Parameters.AddWithValue("@GUID", uid);

                                con1.Open();

                                using (SqlDataReader rdr1 = cmd1.ExecuteReader())
                                {
                                    while (rdr1.Read())
                                    {
                                        DateTime dt = Convert.ToDateTime(rdr1["ResetRequestDateTime"]);
                                        DateTime dtNow = DateTime.Now;

                                        String difference = (dtNow - dt).TotalMinutes.ToString();
                                        double differenceNumeric = double.Parse(difference);

                                        if(differenceNumeric >= 5.0)
                                        {
                                            flag = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return flag;
        }

        public void changePassword(ResetPasswordViewModel resetPasswordVM, string GUID)
        {
            string cs = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spchangePassword", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@GUID", GUID);
                cmd.Parameters.AddWithValue("@Password", resetPasswordVM.Newpassword);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }

    public class UserUtility
    {
        public void SendPasswordResetEmail(string ToEmail, String Username, String UniqueId)
        {
            MailMessage mailMessage = new MailMessage(ConfigurationManager.AppSettings["Username"], ToEmail);

            StringBuilder emailBody = new StringBuilder();
            emailBody.Append("Dear " + Username + ",<br/><br/>");
            emailBody.Append("Please click on the following");
            emailBody.Append("<br/>");
            emailBody.Append("http://localhost:50193/Home/ResetPassword?uid=" + UniqueId);
            emailBody.Append("<br/><br/>");
            emailBody.Append("<b>EVENTHUB</b>");

            mailMessage.IsBodyHtml = true;

            mailMessage.Body = emailBody.ToString();
            mailMessage.Subject = "Reset Your Password";
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = ConfigurationManager.AppSettings["Username"],
                Password = ConfigurationManager.AppSettings["Password"]
            };

            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);
        }
    }
}
