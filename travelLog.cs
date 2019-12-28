using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNetV2
{
    namespace Entities
    {
        enum TravelMode { Bus, Car, Train, Bike}
        class User
        {
            public int UserID { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }

            //public override int GetHashCode()
            //{
            //    return UserID.GetHashCode();
            //}

            //public override string ToString()
            //{
            //    return base.ToString();
            //}
        }

        
        class Travel
        {
            public int TravelID { get; set; }
            public string Destination { get; set; }
            public DateTime DateOfTravel { get; set; }
            public int NoOfDays { get; set; }
            public TravelMode ModeOfTransport { get; set; }
            public string Details { get; set; }
            public User UserDetails { get; set; }
        }
    }
    namespace DataLayer
    {
        using Entities;
        using System.Collections.Generic;

        interface ITravelBlog
        {
            void AddTravelDetails(Travel details);
            void EditTravelInfo(Travel details);
            List<Travel> FindDetails(string destination);
            User CurrentUser { get; set; }
        }

        interface IUserLog
        {
            void RegisterNewUser(User user);
            User Login(string username, string password);
        }

        class UserLog : IUserLog
        {
            Dictionary<int,User> users = new Dictionary<int, User>();
            //HashSet<User> registeredUsers = new HashSet<User>();
            public User Login(string username, string password)
            {
                foreach(var user in users)
                {
                    if (user.Value.UserName.Equals(username) && user.Value.Password.Equals(password))
                        return user.Value;
                }
                return null;
                //throw new Exception("Available in next version");
            }

            public void RegisterNewUser(User user)
            {
                int hashId = user.GetHashCode();
                users.Add(hashId, user);
                //throw new Exception("Available in next version");
            }
        }

        class TravelBlog : ITravelBlog
        {
            private List<Travel> allData = new List<Travel>();

            public User CurrentUser { get; set; }

            public void AddTravelDetails(Travel details)
            {
                allData.Add(details);
            }

            public void EditTravelInfo(Travel details)
            {
                if (details.UserDetails.UserName != CurrentUser.UserName)
                {
                    throw new Exception("You are not authorized to edit the Details");
                }
                for (int i = 0; i < allData.Count; i++)
                {
                    if (allData[i].TravelID == details.TravelID)
                    {
                        allData[i] = details;
                        return;//Exits the function....
                    }
                }
                throw new Exception("No Details of the Travel found to edit");
            }

            public List<Travel> FindDetails(string destination)
            {
                //return allData.FindAll((t) => t.Destination == destination);
                List<Travel> tempList = new List<Travel>();
                foreach (var tr in allData)
                {
                    if (tr.Destination.Contains(destination))
                        tempList.Add(tr);
                }
                return tempList;
            }
        }
    }
    namespace UILayer
    {
        using Entities;
        using DataLayer;
        using System.IO;
        using Frameworks;

        class CompleteExample
        {
            static ITravelBlog blog = new TravelBlog();
            static UserLog userCredentials = new UserLog();
            static CompleteExample()
            {
                //blog.CurrentUser = new User { UserID = 1, UserName = "Phaniraj", Password = "Apple123" };
            }
            static string getMenu(string filename)
            {
                string filedetails = File.ReadAllText(filename);
                return filedetails;
            }
            static void Main(string[] args)
            {
                string contents = getMenu(args[0]);
                bool processing = true;
                do
                {
                    string choice = MyConsole.GetString(contents);
                    processing = processMenu(choice);
                    clearScreen();
                } while (processing);
            }

            private static void clearScreen()
            {
                Console.WriteLine("Press any key to clear");
                Console.ReadKey();
                Console.Clear();
            }

            private static bool processMenu(string choice)
            {
                switch (choice)
                {
                    case "1":
                        {
                            Boolean flag = userLogin();
                            if (!flag)
                                registerUser();
                            return true;
                        }
                    case "2":
                        return true;
                    case "3":
                        addTravelDetails();
                        return true;
                    case "4":
                    case "5":
                        searchFeature();
                        return true;
                    default:
                        return false;
                }
            }

            private static void registerUser()
            {
                int userId = MyConsole.GetNumber("Enter user id: ");
                string username = MyConsole.GetString("Enter username:");
                string password = MyConsole.GetString("Enter password: ");
                userCredentials.RegisterNewUser(new User { UserID = userId, UserName = username, Password = password });
                //throw new NotImplementedException();
            }

            private static Boolean userLogin()
            {
                string username = MyConsole.GetString("Enter username:");
                string password = MyConsole.GetString("Enter password: ");
                var user = userCredentials.Login(username, password);
                blog.CurrentUser = user;
                if (user != null)
                    return true;
                else
                    return false;
            }

            private static void searchFeature()
            {
                string destination = MyConsole.GetString("Enter the Destination or a part of the Destination Name");
                var data = blog.FindDetails(destination);
                foreach (var tr in data)
                    Console.WriteLine(tr.ToString());
            }

            private static void addTravelDetails()
            {
                if (blog.CurrentUser == null)
                {
                    Console.WriteLine("U have not logged in to Post a Travel Info");
                    Console.WriteLine("Please login to add new details");
                    return;
                }
                Travel details = new Travel();
                details.NoOfDays = MyConsole.GetNumber("Enter the No of days of Travel");
                details.ModeOfTransport = MyConsole.GetMode();
                details.DateOfTravel = MyConsole.GetDate("Enter the date as dd/MM/yyyy");
                details.Destination = MyConsole.GetString("Enter UR place of Visit");
                details.UserDetails = blog.CurrentUser;
                details.Details = MyConsole.GetString("Please enter UR Experience in the Travel to be shared to all our visitors");
                try
                {
                    blog.AddTravelDetails(details);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
