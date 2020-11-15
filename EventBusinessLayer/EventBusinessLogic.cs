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
using Humanizer;

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
                Regex pattern = new Regex(@"^[a-zA-Z\s]+$");
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
                else
                {
                    // Greater than todays date
                    DateTime today = DateTime.Today;
                    DateTime eventTime = DateTime.Parse(newEvent.Date);

                    if(!(eventTime >= today))
                    {
                        mState.AddModelError("Date", "Date entered is already passed");
                        modelStateFlag = false;
                    }
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
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadedEventImages"), _FileName);
                            newEvent.Image.SaveAs(path);

                            cmd.Parameters.AddWithValue("@EventImage", path);
                        }
                    }
                    catch
                    {
                        string _FileName = "Default.jpg";
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadedEventImages"), _FileName);

                        cmd.Parameters.AddWithValue("@EventImage", path);
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

        public List<EventCard> getAllEvents()
        {
            List<EventCard> eventCards = new List<EventCard>();

            string cs = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spGetAllEvents", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while(rdr.Read())
                    {
                        EventCard eventCard = new EventCard();

                        eventCard.EventId = (int)rdr["Id"];

                        eventCard.Title = rdr["Title"].ToString();

                        char[] separators = { '/', ' ' };
                        string[] dateBits = rdr["EventDate"].ToString().Split(separators);
                        eventCard.Date = new DateTime(int.Parse(dateBits[2]), int.Parse(dateBits[0]), int.Parse(dateBits[1])).ToOrdinalWords();

                        string[] timeBits = rdr["EventTime"].ToString().Split(':');
                        string postFix = "";
                        if (int.Parse(timeBits[0]) == 11)
                        {
                            postFix = "AM";
                        }
                        else
                        {
                            postFix = (int.Parse(timeBits[0]) / 11) == 0 ? "AM" : "PM";
                        }
                        eventCard.Time = timeBits[0] + "." + timeBits[1] + " " + postFix;

                        string imageRaw = rdr["EventImage"].ToString();
                        int index = imageRaw.LastIndexOf('\\');
                        eventCard.Image = imageRaw.Substring(index+1);

                        eventCards.Add(eventCard);
                    }

                }
            }

            return eventCards;
        }

        public List<EventCard> getAllEvents(string catergory)
        {
            string cs = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;
            int eventId;

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spGetCatergoryId", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Catergory", catergory);

                SqlParameter outputParameter = new SqlParameter();
                outputParameter.ParameterName = "@Id";
                outputParameter.SqlDbType = SqlDbType.Int;
                outputParameter.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(outputParameter);

                con.Open();
                cmd.ExecuteScalar();

                eventId = (int)outputParameter.Value;
            }

            List<EventCard> eventCards = new List<EventCard>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spGetEventByCatergory", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", eventId);

                con.Open();

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        EventCard eventCard = new EventCard();

                        eventCard.EventId = (int)rdr["Id"];

                        eventCard.Title = rdr["Title"].ToString();

                        char[] separators = { '/', ' ' };
                        string[] dateBits = rdr["EventDate"].ToString().Split(separators);
                        eventCard.Date = new DateTime(int.Parse(dateBits[2]), int.Parse(dateBits[0]), int.Parse(dateBits[1])).ToOrdinalWords();

                        string[] timeBits = rdr["EventTime"].ToString().Split(':');
                        string postFix = "";
                        if (int.Parse(timeBits[0]) == 11)
                        {
                            postFix = "AM";
                        }
                        else
                        {
                            postFix = (int.Parse(timeBits[0]) / 11) == 0 ? "AM" : "PM";
                        }
                        eventCard.Time = timeBits[0] + "." + timeBits[1] + " " + postFix;

                        string imageRaw = rdr["EventImage"].ToString();
                        int index = imageRaw.LastIndexOf('\\');
                        eventCard.Image = imageRaw.Substring(index + 1);

                        eventCards.Add(eventCard);
                    }

                }
            }

            return eventCards;
        }

        public List<EventInfo> getSearchResults(string searchValue)
        {
            List<EventInfo> eventInfoList = new List<EventInfo>();

            if (!String.IsNullOrEmpty(searchValue))
            {

                string cs = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;
                using (SqlConnection con = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand("spSearchResults", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@searchValue", searchValue);

                    con.Open();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            EventInfo eventInfo = new EventInfo();

                            eventInfo.Id = (int)rdr["Id"];

                            eventInfo.Title = rdr["Title"].ToString();

                            char[] separators = { '/', ' ' };
                            string[] dateBits = rdr["EventDate"].ToString().Split(separators);
                            eventInfo.Date = new DateTime(int.Parse(dateBits[2]), int.Parse(dateBits[0]), int.Parse(dateBits[1])).ToOrdinalWords();

                            string[] timeBits = rdr["EventTime"].ToString().Split(':');
                            string postFix = "";
                            if (int.Parse(timeBits[0]) == 11)
                            {
                                postFix = "AM";
                            }
                            else
                            {
                                postFix = (int.Parse(timeBits[0]) / 11) == 0 ? "AM" : "PM";
                            }
                            eventInfo.Time = timeBits[0] + "." + timeBits[1] + " " + postFix;

                            string imageRaw = rdr["EventImage"].ToString();
                            int index = imageRaw.LastIndexOf('\\');
                            eventInfo.Image = imageRaw.Substring(index + 1);

                            eventInfo.Description = rdr["EventDescription"].ToString();

                            eventInfo.Location = rdr["EventLocation"].ToString();

                            eventInfoList.Add(eventInfo);
                        }
                    }
                }
            }

            return eventInfoList;
        }

        public List<EventInfo> getBrowseResults(string browseType)
        {
            List<EventInfo> eventInfoList = new List<EventInfo>();

            string cs = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;

            int eventId = 0;

            if (browseType != "All")
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand("spGetCatergoryId", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Catergory", browseType);

                    SqlParameter outputParameter = new SqlParameter();
                    outputParameter.ParameterName = "@Id";
                    outputParameter.SqlDbType = SqlDbType.Int;
                    outputParameter.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(outputParameter);

                    con.Open();
                    cmd.ExecuteScalar();

                    eventId = (int)outputParameter.Value;
                }
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd;

                if (browseType != "All")
                {
                    cmd = new SqlCommand("spGetEventByCatergory", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", eventId);
                }
                else
                {
                    cmd = new SqlCommand("spGetAllEvents", con);
                }

                con.Open();

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        EventInfo eventInfo = new EventInfo();

                        eventInfo.Id = (int)rdr["Id"];

                        eventInfo.Title = rdr["Title"].ToString();

                        char[] separators = { '/', ' ' };
                        string[] dateBits = rdr["EventDate"].ToString().Split(separators);
                        eventInfo.Date = new DateTime(int.Parse(dateBits[2]), int.Parse(dateBits[0]), int.Parse(dateBits[1])).ToOrdinalWords();

                        string[] timeBits = rdr["EventTime"].ToString().Split(':');
                        string postFix = "";
                        if (int.Parse(timeBits[0]) == 11)
                        {
                            postFix = "AM";
                        }
                        else
                        {
                            postFix = (int.Parse(timeBits[0]) / 11) == 0 ? "AM" : "PM";
                        }
                        eventInfo.Time = timeBits[0] + "." + timeBits[1] + " " + postFix;

                        string imageRaw = rdr["EventImage"].ToString();
                        int index = imageRaw.LastIndexOf('\\');
                        eventInfo.Image = imageRaw.Substring(index + 1);

                        eventInfo.Description = rdr["EventDescription"].ToString();

                        eventInfoList.Add(eventInfo);
                    }
                }
            }

            return eventInfoList;
        }

        public List<EventInfo> getDTResults(EventDT eventDT)
        {
            List<EventInfo> eventInfoList = new List<EventInfo>();

            if(!string.IsNullOrEmpty(eventDT.FromDate) || !string.IsNullOrEmpty(eventDT.ToDate) || !string.IsNullOrEmpty(eventDT.FromTime) || !string.IsNullOrEmpty(eventDT.ToTime))
            {
                string cs = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;

                if (!string.IsNullOrEmpty(eventDT.FromDate) && !string.IsNullOrEmpty(eventDT.ToDate) && !string.IsNullOrEmpty(eventDT.FromTime) && !string.IsNullOrEmpty(eventDT.ToTime))
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        SqlCommand cmd = new SqlCommand("spFTdateFTtime", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fromDate", eventDT.FromDate);
                        cmd.Parameters.AddWithValue("@toDate", eventDT.ToDate);
                        cmd.Parameters.AddWithValue("@fromTime", eventDT.FromTime);
                        cmd.Parameters.AddWithValue("@toTime", eventDT.ToTime);

                        con.Open();

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while(rdr.Read())
                            {
                                EventInfo eventInfo = new EventInfo();

                                eventInfo.Id = (int)rdr["Id"];

                                eventInfo.Title = rdr["Title"].ToString();

                                char[] separators = { '/', ' ' };
                                string[] dateBits = rdr["EventDate"].ToString().Split(separators);
                                eventInfo.Date = new DateTime(int.Parse(dateBits[2]), int.Parse(dateBits[0]), int.Parse(dateBits[1])).ToOrdinalWords();

                                string[] timeBits = rdr["EventTime"].ToString().Split(':');
                                string postFix = "";
                                if (int.Parse(timeBits[0]) == 11)
                                {
                                    postFix = "AM";
                                }
                                else
                                {
                                    postFix = (int.Parse(timeBits[0]) / 11) == 0 ? "AM" : "PM";
                                }
                                eventInfo.Time = timeBits[0] + "." + timeBits[1] + " " + postFix;

                                string imageRaw = rdr["EventImage"].ToString();
                                int index = imageRaw.LastIndexOf('\\');
                                eventInfo.Image = imageRaw.Substring(index + 1);

                                eventInfo.Description = rdr["EventDescription"].ToString();

                                eventInfoList.Add(eventInfo);
                            }
                        }
                    }
                }
                else if(!string.IsNullOrEmpty(eventDT.FromDate) && !string.IsNullOrEmpty(eventDT.ToDate))
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        SqlCommand cmd = new SqlCommand("spFTdate", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fromDate", eventDT.FromDate);
                        cmd.Parameters.AddWithValue("@toDate", eventDT.ToDate);

                        con.Open();

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                EventInfo eventInfo = new EventInfo();

                                eventInfo.Id = (int)rdr["Id"];

                                eventInfo.Title = rdr["Title"].ToString();

                                char[] separators = { '/', ' ' };
                                string[] dateBits = rdr["EventDate"].ToString().Split(separators);
                                eventInfo.Date = new DateTime(int.Parse(dateBits[2]), int.Parse(dateBits[0]), int.Parse(dateBits[1])).ToOrdinalWords();

                                string[] timeBits = rdr["EventTime"].ToString().Split(':');
                                string postFix = "";
                                if (int.Parse(timeBits[0]) == 11)
                                {
                                    postFix = "AM";
                                }
                                else
                                {
                                    postFix = (int.Parse(timeBits[0]) / 11) == 0 ? "AM" : "PM";
                                }
                                eventInfo.Time = timeBits[0] + "." + timeBits[1] + " " + postFix;

                                string imageRaw = rdr["EventImage"].ToString();
                                int index = imageRaw.LastIndexOf('\\');
                                eventInfo.Image = imageRaw.Substring(index + 1);

                                eventInfo.Description = rdr["EventDescription"].ToString();

                                eventInfoList.Add(eventInfo);
                            }
                        }
                    }
                }
                else if(!string.IsNullOrEmpty(eventDT.FromTime) && !string.IsNullOrEmpty(eventDT.ToTime))
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        SqlCommand cmd = new SqlCommand("spFTtime", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fromTime", eventDT.FromTime);
                        cmd.Parameters.AddWithValue("@toTime", eventDT.ToTime);

                        con.Open();

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                EventInfo eventInfo = new EventInfo();

                                eventInfo.Id = (int)rdr["Id"];

                                eventInfo.Title = rdr["Title"].ToString();

                                char[] separators = { '/', ' ' };
                                string[] dateBits = rdr["EventDate"].ToString().Split(separators);
                                eventInfo.Date = new DateTime(int.Parse(dateBits[2]), int.Parse(dateBits[0]), int.Parse(dateBits[1])).ToOrdinalWords();

                                string[] timeBits = rdr["EventTime"].ToString().Split(':');
                                string postFix = "";
                                if (int.Parse(timeBits[0]) == 11)
                                {
                                    postFix = "AM";
                                }
                                else
                                {
                                    postFix = (int.Parse(timeBits[0]) / 11) == 0 ? "AM" : "PM";
                                }
                                eventInfo.Time = timeBits[0] + "." + timeBits[1] + " " + postFix;

                                string imageRaw = rdr["EventImage"].ToString();
                                int index = imageRaw.LastIndexOf('\\');
                                eventInfo.Image = imageRaw.Substring(index + 1);

                                eventInfo.Description = rdr["EventDescription"].ToString();

                                eventInfoList.Add(eventInfo);
                            }
                        }
                    }
                }
                else if(!string.IsNullOrEmpty(eventDT.FromDate) && !string.IsNullOrEmpty(eventDT.FromTime))
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        SqlCommand cmd = new SqlCommand("spFdateFtime", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fromDate", eventDT.FromDate);
                        cmd.Parameters.AddWithValue("@fromTime", eventDT.FromTime);

                        con.Open();

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                EventInfo eventInfo = new EventInfo();

                                eventInfo.Id = (int)rdr["Id"];

                                eventInfo.Title = rdr["Title"].ToString();

                                char[] separators = { '/', ' ' };
                                string[] dateBits = rdr["EventDate"].ToString().Split(separators);
                                eventInfo.Date = new DateTime(int.Parse(dateBits[2]), int.Parse(dateBits[0]), int.Parse(dateBits[1])).ToOrdinalWords();

                                string[] timeBits = rdr["EventTime"].ToString().Split(':');
                                string postFix = "";
                                if (int.Parse(timeBits[0]) == 11)
                                {
                                    postFix = "AM";
                                }
                                else
                                {
                                    postFix = (int.Parse(timeBits[0]) / 11) == 0 ? "AM" : "PM";
                                }
                                eventInfo.Time = timeBits[0] + "." + timeBits[1] + " " + postFix;

                                string imageRaw = rdr["EventImage"].ToString();
                                int index = imageRaw.LastIndexOf('\\');
                                eventInfo.Image = imageRaw.Substring(index + 1);

                                eventInfo.Description = rdr["EventDescription"].ToString();

                                eventInfoList.Add(eventInfo);
                            }
                        }
                    }
                }
                else if(!string.IsNullOrEmpty(eventDT.FromDate))
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        SqlCommand cmd = new SqlCommand("spFdate", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fromDate", eventDT.FromDate);

                        con.Open();

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                EventInfo eventInfo = new EventInfo();

                                eventInfo.Id = (int)rdr["Id"];

                                eventInfo.Title = rdr["Title"].ToString();

                                char[] separators = { '/', ' ' };
                                string[] dateBits = rdr["EventDate"].ToString().Split(separators);
                                eventInfo.Date = new DateTime(int.Parse(dateBits[2]), int.Parse(dateBits[0]), int.Parse(dateBits[1])).ToOrdinalWords();

                                string[] timeBits = rdr["EventTime"].ToString().Split(':');
                                string postFix = "";
                                if (int.Parse(timeBits[0]) == 11)
                                {
                                    postFix = "AM";
                                }
                                else
                                {
                                    postFix = (int.Parse(timeBits[0]) / 11) == 0 ? "AM" : "PM";
                                }
                                eventInfo.Time = timeBits[0] + "." + timeBits[1] + " " + postFix;

                                string imageRaw = rdr["EventImage"].ToString();
                                int index = imageRaw.LastIndexOf('\\');
                                eventInfo.Image = imageRaw.Substring(index + 1);

                                eventInfo.Description = rdr["EventDescription"].ToString();

                                eventInfoList.Add(eventInfo);
                            }
                        }
                    }
                }
                else if(!string.IsNullOrEmpty(eventDT.FromTime))
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        SqlCommand cmd = new SqlCommand("spFtime", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fromTime", eventDT.FromTime);

                        con.Open();

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                EventInfo eventInfo = new EventInfo();

                                eventInfo.Id = (int)rdr["Id"];

                                eventInfo.Title = rdr["Title"].ToString();

                                char[] separators = { '/', ' ' };
                                string[] dateBits = rdr["EventDate"].ToString().Split(separators);
                                eventInfo.Date = new DateTime(int.Parse(dateBits[2]), int.Parse(dateBits[0]), int.Parse(dateBits[1])).ToOrdinalWords();

                                string[] timeBits = rdr["EventTime"].ToString().Split(':');
                                string postFix = "";
                                if (int.Parse(timeBits[0]) == 11)
                                {
                                    postFix = "AM";
                                }
                                else
                                {
                                    postFix = (int.Parse(timeBits[0]) / 11) == 0 ? "AM" : "PM";
                                }
                                eventInfo.Time = timeBits[0] + "." + timeBits[1] + " " + postFix;

                                string imageRaw = rdr["EventImage"].ToString();
                                int index = imageRaw.LastIndexOf('\\');
                                eventInfo.Image = imageRaw.Substring(index + 1);

                                eventInfo.Description = rdr["EventDescription"].ToString();

                                eventInfoList.Add(eventInfo);
                            }
                        }
                    }
                }
            }

            return eventInfoList;
        }

        public EventInfo getEvent(int Id)
        {
            EventInfo eventInfo = new EventInfo();

            string cs = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spGetEvent", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", Id);

                con.Open();

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while(rdr.Read())
                    {
                        eventInfo.Id = (int)rdr["Id"];

                        eventInfo.Title = rdr["Title"].ToString();

                        char[] separators = { '/', ' ' };
                        string[] dateBits = rdr["EventDate"].ToString().Split(separators);
                        eventInfo.Date = new DateTime(int.Parse(dateBits[2]), int.Parse(dateBits[0]), int.Parse(dateBits[1])).ToOrdinalWords();

                        string[] timeBits = rdr["EventTime"].ToString().Split(':');
                        string postFix = "";
                        if (int.Parse(timeBits[0]) == 11)
                        {
                            postFix = "AM";
                        }
                        else
                        {
                            postFix = (int.Parse(timeBits[0]) / 11) == 0 ? "AM" : "PM";
                        }
                        eventInfo.Time = timeBits[0] + "." + timeBits[1] + " " + postFix;

                        string imageRaw = rdr["EventImage"].ToString();
                        int index = imageRaw.LastIndexOf('\\');
                        eventInfo.Image = imageRaw.Substring(index + 1);

                        eventInfo.Description = rdr["EventDescription"].ToString();

                        eventInfo.Location = rdr["EventLocation"].ToString();
                    }
                }
            }

            return eventInfo;
        }
    }
}
