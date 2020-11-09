using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBusinessLayer.ViewModels;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.IO;
using System.Web;

namespace EventBusinessLayer
{
    public class EventBusinessLogic
    {
        public bool addNewEvent(EventViewModel newEvent, ModelStateDictionary mState, string sessionTag)
        {
            bool modelStateFlag = true;
            
            // Title
            if(String.IsNullOrEmpty(newEvent.Title))
            {
                mState.AddModelError("Title", "Title cannot be empty");
                modelStateFlag = false;
            }

            // Catergory
            if(String.IsNullOrEmpty(newEvent.Catergories))
            {
                mState.AddModelError("Catergories", "Please select a catergory for the event");
                modelStateFlag = false;
            }

            // Location
            if (String.IsNullOrEmpty(newEvent.Location))
            {
                mState.AddModelError("Location", "Location cannot be empty");
                modelStateFlag = false;
            }
            else
            {
                Regex pattern = new Regex(@"^[a-zA-Z]+$");
                if(!pattern.IsMatch(newEvent.Location))
                {
                    mState.AddModelError("Location", "Invalid character/s entered for location");
  
                      modelStateFlag = false;
                }
            }

            // Time
            if (String.IsNullOrEmpty(newEvent.Time))
            {
                mState.AddModelError("Time", "Time cannot be empty");
                modelStateFlag = false;
            }
            else
            {
                Regex pattern = new Regex(@"^[0-2]{1}[0-9]{1}:[0-5]{1}[0-9]{1}$");
                if (!pattern.IsMatch(newEvent.Time))
                {
                    mState.AddModelError("Time", "Please follow the format HH:MM");

                    modelStateFlag = false;
                }
            }

            // Date
            if (String.IsNullOrEmpty(newEvent.Date))
            {
                mState.AddModelError("Date", "Date cannot be empty");
                modelStateFlag = false;
            }
            else
            {
                Regex pattern = new Regex(@"^[0-1]{1}[0-2]{1}\/[0-3]{1}[0-9]{1}\/[0-2]{1}[0-9]{1}[0-9]{1}[0-9]{1}$");
                if (!pattern.IsMatch(newEvent.Date))
                {
                    mState.AddModelError("Date", "Please follow the format MM/DD/YYYY");

                    modelStateFlag = false;
                }
            }

            if(modelStateFlag)
            {
                string cs = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;
                using (SqlConnection con = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand("spAddNewEvent", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Title", newEvent.Title);
                    cmd.Parameters.AddWithValue("@EventCatergoryId", int.Parse(newEvent.Catergories));
                    cmd.Parameters.AddWithValue("@EventLocation", newEvent.Location);
                    cmd.Parameters.AddWithValue("@EventTime", newEvent.Time);
                    cmd.Parameters.AddWithValue("@EventDate", newEvent.Date);
                    cmd.Parameters.AddWithValue("@EventDescription", newEvent.Description);

                    // Image logic
                    try
                    {
                        if (newEvent.Image.ContentLength > 0)
                        {
                            string _FileName = Path.GetFileName(newEvent.Image.FileName);
                            string _path = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadedEventImages"), _FileName);
                            newEvent.Image.SaveAs(_path);

                            cmd.Parameters.AddWithValue("@EventImage", _path);
                        }
                    }
                    catch
                    {
                        cmd.Parameters.AddWithValue("@EventImage", "~/UploadedEventImages/Default.jpg");
                    }

                    int UserID;
                    // Get current user id from Email
                    if (sessionTag.Contains("@"))
                    {
                        using (SqlConnection connection = new SqlConnection(cs))
                        {
                            SqlCommand command = new SqlCommand("spGetUserIDFromEmail", connection);
                            command.CommandType = System.Data.CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@Email", sessionTag);

                            SqlParameter outputParameter = new SqlParameter();
                            outputParameter.ParameterName = "@ID";
                            outputParameter.SqlDbType = SqlDbType.Int;
                            outputParameter.Direction = ParameterDirection.Output;

                            command.Parameters.Add(outputParameter);

                            connection.Open();
                            command.ExecuteScalar();

                            UserID = (int)outputParameter.Value;
                        }
                    }
                    else
                    {
                        using (SqlConnection connection = new SqlConnection(cs))
                        {
                            SqlCommand command = new SqlCommand("spGetUserID", connection);
                            command.CommandType = System.Data.CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@Username", sessionTag);

                            SqlParameter outputParameter = new SqlParameter();
                            outputParameter.ParameterName = "@ID";
                            outputParameter.SqlDbType = SqlDbType.Int;
                            outputParameter.Direction = ParameterDirection.Output;

                            command.Parameters.Add(outputParameter);

                            connection.Open();
                            command.ExecuteScalar();

                            UserID = (int)outputParameter.Value;
                        }
                    }

                    cmd.Parameters.AddWithValue("@UserId", UserID);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return modelStateFlag;
        }

        public List<SelectListItem> getCatergoryList()
        {
            List<SelectListItem> catergories = new List<SelectListItem>();

            string cs = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spGetCatergories", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while(rdr.Read())
                    {
                        SelectListItem catergory = new SelectListItem
                        {
                            Text = rdr["Catergory"].ToString(),
                            Value = rdr["Id"].ToString()
                        };
                        catergories.Add(catergory);
                    }
                }

            }

             return catergories;
        }
    }
}
