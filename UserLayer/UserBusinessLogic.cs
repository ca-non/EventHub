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
            else if(newUser.UserName.Length > 200)
            {
                mState.AddModelError("UserName", "Username is too long");
                modelStateFlag = false;
            }
            else
            {
                Regex pattern = new Regex(@"^\w+$");
                if(!pattern.IsMatch(newUser.UserName))
                {
                    mState.AddModelError("UserName", "Username can contain only letters, numbers and _ ");
                    modelStateFlag = false;
                }
                
            }

            // Password server validation
            if (String.IsNullOrEmpty(newUser.Passwd) || newUser.Passwd.Length > 200 || newUser.Passwd.Length < 6)
            {
                if(String.IsNullOrEmpty(newUser.Passwd))
                {
                    mState.AddModelError("Passwd", "Password is required");
                    modelStateFlag = false;
                }
                else if(newUser.Passwd.Length > 200)
                {
                    mState.AddModelError("passwd", "Password is too long");
                    modelStateFlag =  false;
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
            if(!String.IsNullOrEmpty(newUser.Passwd) && newUser.Passwd != newUser.ConfirmPasswd)
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
                if(!pattern.IsMatch(newUser.Email))
                {
                    mState.AddModelError("Email", "Invalid email entered");
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
                string Email = "";
                string Username = "";
                string connectionString = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;
                int count = 0;

                if (currentUser.UsernameEmail.Contains("@"))
                {
                    Email = currentUser.UsernameEmail;

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlCommand cmd = new SqlCommand("spCheckUserForEmailr", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    if(count == 1)
                    {
                        // check for the correspoding password
                        // if for password error
                    }
                    else
                    {
                        // flag to flase
                        // add model error - username or password is incorrect
                    }
                }
                else
                {
                    Username = currentUser.UsernameEmail;

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlCommand cmd = new SqlCommand("spCheckUserUsername", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    if (count == 1)
                    {
                        // check for the correspoding password
                        // if for password error
                    }
                    else
                    {
                        // flag to flase
                        // add model error - username or password is incorrect
                    }
                }

            }


            return modelStateFlag;
        }
    }
}
