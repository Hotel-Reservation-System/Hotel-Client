/* CHALLENGE: Hotel Reservation System - Create a reservation system which books hotel rooms. It 
 * charges various rates for particular rooms of the hotel. Example, hotel rooms have penthouse 
 * suites which cost more. Keep track of when rooms will be available and can be scheduled.
 *
 * 
 * IMPLEMENTATION GOALS
 * 
 * 1) Create a database with with tables to track:
 * 
 *      1. Hotels,
 *      2. Hotel Rooms, 
 *      3. The Occupancy Status for each Room, etc. 
 *      
 *    The second table in this database will reference the first table via a foreign key. The 
 *    third table will reference the first two tables. By doing this, you can track hotel rooms and 
 *    vacancy status across many hotels.
 *    
 * 2) Another option would be to add a fake authentication system. In this authentication system,
 *    hardcode two users in the app, an hotel admin account and a general user account. The admin  
 *    account is for hotel employees and the user account is for hotel customers. When the app 
 *    starts up it asks for a username and password. The logic will be in a while loop.
 *
 *
 ***************************************************************************************************
 ***************************************************************************************************
 *
 * 
 * PROGRAM ARCHITECTURE
 *
 * This program, Hotel Reservation System Program (HRSP), is divided into 3 separate projects,
 * Hotel-Client, Hotel-Server and Common. It is broken down into the typical multi-tier
 * architecture to separate the Presentation, Business logic and Data Access layers. See below for
 * more details.
 *
 * 1. HOTEL-CLIENT: is the presentation layer of this program. It is a console application that 
 *    presents a text-based UI to authenticate users and determines their level of access to the 
 *    database. Once authenticated, users can issue requests to Hotel-Server for information from
 *    the database.
 * 
 * 2. HOTEL-SERVER: is the data access layer (DAL) that contains the database. Hotel-Server was 
 *    carved out of the original monolithic project because the DAL should be program and language 
 *    agnostic. It is a WebAPI project that is accessible independent of Hotel-Client. The DAL is 
 *    partially structured in the Model-View-Controller (MVC) architectural pattern. As
 *    Hotel-Server is only an API, it doesn't NEED to implement Views, only Models and
 *    Controllers. It gets the models (M) from the Common project and it implements Controllers (C).
 *    The Controllers use the DAL to modify the Models in the database, and then updates the Views.
 * 
 * 3. COMMON: is the business logic layer of the program. It contains model classes that will be
 *    consumed by, and thus is common to the other two projects. Common will compile to a .dll file
 *    and a copy of it will be packaged with both HotelApi and Hotel Reservation Client.
 *
 *
 * MULTI-TIER ARCHITECTURE VS. MVC ARCHITECTURE
 *
 * The Multi-tier architecture is designed for client-server architectures, not just for a single
 * application architecture. This architecture splits an application into different layers, in turn,
 * creating different applications and services. Here's a short-introduction to the client-server
 * architecture:
 *
 *     Client-server architecture (client/server) is a network architecture in which each computer
 *     or process on the network is either a client or a server.
 *
 *     Servers are powerful computers or processes dedicated to managing disk drives (file servers),
 *     printers (print servers), or network traffic (network servers). Clients are PCs or
 *     workstations on which users run applications. Clients rely on servers for resources, such as
 *     files, devices, and even processing power.
 *     (Source: https://www.webopedia.com/TERM/C/client_server_architecture.html)
 *
 * In the HRSP, the Hotel-Client is the client program. This console program runs client side and 
 * every end-user runs an instance of this program on their computer. The DAL project, Hotel-Server,
 * is the server-side program, which will provide database access to all authorized clients.
 * There will be only one instance (or a few) of Hotel-Server, but many running instances of 
 * Hotel-Client. Common is a class library that will compile down to a .dll file and it will be 
 * included with both Hotel-Client and Hotel-Server.
 *
 * MVC is a better architecture for a single application. The main purpose is to separate the
 * application logic from the user interface. MVC and multi-tier architectures are not exclusive;
 * you aren't limited to choosing one or the other. HRSP uses both architectures. The Hotel-Server,
 * Hotel-Client and Common make up the multi-tier architecture. Note that HotelApi is a WebAPI and 
 * as such, it does not need to implement Views (V). What the HotelApi project does is implement
 * Controllers (C) while getting its Models (M) from the Common project.
 *
 * See the Hotel-Server Project for more information about MVC.
 */


using System;
using Common.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Hotel_Client
{
    class Program
    {
        static HttpClient client = new HttpClient();


        static async Task MainAsync(string[] args)
        {
            Console.WriteLine("This is a Hotel Management System. It has a database with tables to track Hotels, hotel rooms and room status respectively.");

            // Login to an account, either a Limited user account or an Admin account.
            // Check the UserAuthenticator class for the credentials.
            Console.WriteLine("Please login to an account first.");
            LoginToDatabase();
            string AccountType = UserAuthenticator.LoggedInUser;

            int userChoice;

            if (AccountType == "admin")
            {
                userChoice = ShowAdminMenu();
            }
            if (AccountType == "user")
            {
                userChoice = ShowUserMenu();
            }

            // Declaring the URL of the local server. 
            client.BaseAddress = new Uri("http://localhost:29298/api/hotel");
            client.DefaultRequestHeaders.Accept.
                Add(new MediaTypeWithQualityHeaderValue("application/json"));



            // Connect to Database Method.
            List<Hotel> listOfHotels = await GetHotelsAsync();
        }


        // Logs into one of the two accounts.
        private static void LoginToDatabase()
        {
            bool loginStatus = false;

            while (loginStatus == false)
            {
                Console.WriteLine();

                Console.Write("Enter the username: ");
                string userName = Console.ReadLine();

                Console.Write("Please enter the password: ");
                string password = Console.ReadLine();

                loginStatus = UserAuthenticator.LoginToAccount(userName, password);
                Console.WriteLine("Credentials are Incorrect. Try Again.");
            }
        }


        // Show the User Administrative Options:
        private static int ShowAdminMenu()
        {
            /* ADMINISTRATIVE USER OPTIONS:
             * 
             * 00. Exit Program. 
             * 
             * RETRIEVE INFORMATION
             * 01. Retrieve the List of Hotels from the Database.
             * 02. Retrieve Information about a Hotel from the Database.
             * 03. Retrieve the List of Hotel Rooms from the Database.
             * 04. Retrieve Information about a Hotel Room from the Database.
             * 05. Retrieve Information about a Room Reservation from the Database.
             * 
             * UPDATE INFORMATION
             * 06. Update Information about a Hotel. 
             * 07. Update Information about a Hotel Room. 
             * 08. Update Information about a Room Reservation. 
             * 
             * ADD INFORMATION (Will need to add methods to do this)
             * 09. Add a Hotel to the Database.
             * 10. Add a Hotel Room to a Hotel in the Database.
             * 11. Add a Hotel Room to a Hotel in the Database.
             * 
             * DELETE INFORMATION
             * 12. Delete a Hotel from the Database.
             * 13. Delete a Hotel Room from a Hotel in the Database.
             * 14. Delete a Room Reservation from a Hotel in the Database.
             * 
             * UNIMPLEMENTED POSSIBILITIES
             * 00. Exit Program. 
             * 
             * RETRIEVE INFORMATION
             * 01. Retrieve the List of Hotels from the Database.
             * 02. Retrieve Information about a Hotel from the Database.
             * 03. Retrieve the List of Hotel Rooms from the Database.
             * 04. Retrieve Information about a Hotel Room from the Database.
             * 05. Retrieve Information about a Room Reservation from the Database.          
             */

            string menuOptions = String.Format("Please Choose a Course of Action and Enter its Two Digit Code: {0}00. Exit Program.{0}{0}RETRIEVE INFORMATION {0}{0}01. Retrieve the List of Hotels from the Database. {0}02. Retrieve Information about a Hotel from the Database. {0}03. Retrieve the List of Hotel Rooms from the Database. {0}04. Retrieve Information about a Hotel Room from the Database. {0}05. Retrieve Information about a Room Reservation from the Database.{0}{0}UPDATE INFORMATION {0}{0}06. Update Information about a Hotel. {0}07. Update Information about a Hotel Room. {0}08. Update Information about a Room Reservation. {0}{0} ADD INFORMATION {0}{0}09. Add a Hotel to the Database. {0}10. Add a Hotel Room to a Hotel in the Database. {0}11. Add a Hotel Room to a Hotel in the Database. {0}{0} DELETE INFORMATION {0}{0}12. Delete a Hotel from the Database. {0}13. Delete a Hotel Room from a Hotel in the Database. {0}14. Delete a Room Reservation from a Hotel in the Database.", Environment.NewLine);
            Console.WriteLine(menuOptions);
            int.TryParse(Console.ReadLine(), out int userChoice);

            return userChoice;
        }


        private static int ShowUserMenu()
        {
            /* LIMITIED USER OPTIONS:
             * 
             * 1. Retrieve the List of Hotels from the Database.
             * 2. Retrieve the List of All Rooms at a Hotel. 
             * 3. Retrieve a List of All Unoccupied Rooms at a Hotel.
             * 4. Retrieve a List of All Unoccupied Single-bed Rooms at a Hotel.
             * 5. Retrieve a List of All Unoccupied Multi-bed Rooms at a Hotel.
             */

            string menuOptions = String.Format("Please Choose a Course of Action and Enter its Two Digit Code: {0}00. Exit Program.{0}{0}RETRIEVE INFORMATION {0}01. Retrieve the List of Hotels from the Database. {0}02. Retrieve Information about a Hotel from the Database. {0}03. Retrieve the List of Hotel Rooms from the Database. {0}04. Retrieve Information about a Hotel Room from the Database. {0}05. Retrieve Information about a Room Reservation from the Database.", Environment.NewLine);
            Console.WriteLine(menuOptions);
            int.TryParse(Console.ReadLine(), out int userChoice);

            return userChoice;
        }


        // 01. Connects to Hotel-Server and retrieves a List of ALL Hotels from the Database.
        public static async Task<List<Hotel>> GetHotelsAsync()
        {
            List<Hotel> hotels = null;
            HttpResponseMessage response = 
                await client.GetAsync("http://localhost:29298/api/hotel");

            if (response.IsSuccessStatusCode)
            {
                string hotelListString = await response.Content.ReadAsStringAsync();
                hotels = JsonConvert.DeserializeObject<List<Hotel>>(hotelListString);
            }

            return hotels;
        }


        // Retrieves the List of Hotels from the Database.
        public static IList<Hotel> RetrieveHotels()
        {
            using (var hotelDB = new SQLiteConnection(_connectionString))
            {
                return hotelDB.Table<Hotel>().ToList();
            }
        }


        // Add a Hotel to the Database.
        public static void AddHotel(Hotel hotel)
        {
            using (var hotelDB = new SQLiteConnection(_connectionString))
            {

                hotelDb.RunInTransaction(() =>
                {
                    hotelDb.InsertAll(hotel);
                });
            }
        }


        // Retrieve the List of All Rooms at a Hotel. 
        public static IList<HotelRoom> RetrieveAllHotelRooms(Hotel hotel)
        {
            using (var hotelDB = new SQLiteConnection(_connectionString))
            {
                // Find Hotel, then return its HotelRooms.
                // return hotelDB.Table<Hotel>().ToList();
            }
        }


        // Add a Room to a Hotel.
        public static void AddRoomToHotel(Hotel hotel, HotelRoom hotelRoom)
        {
            using (var hotelDB = new SQLiteConnection(_connectionString))
            {

                hotelDb.RunInTransaction(() =>
                {
                    // Find Hotel, Insert HotelRoom.
                    // hotelDb.InsertAll(hotel);
                });
            }
        }


        // Retrieve a List of All Occupied Rooms at a Hotel.
        public static IList<HotelRoom> RetrieveOccupiedHotelRooms(Hotel hotel)
        {
            using (var hotelDB = new SQLiteConnection(_connectionString))
            {
                // Find Hotel, then return a list of all its HotelRooms.
                // return hotelDB.Table<Hotel>().ToList();
            }
        }


        // Retrieve a List of All Unoccupied Rooms at a Hotel.
        public static IList<HotelRoom> RetrieveUnoccupiedHotelRooms(Hotel hotel)
        {
            using (var hotelDB = new SQLiteConnection(_connectionString))
            {
                // Find Hotel, then return a list  of its unoccupied HotelRooms.
                // return hotelDB.Table<Hotel>().ToList();
            }
        }


        // Retrieve a List of All Unoccupied Single-bed Rooms at a Hotel.
        public static IList<HotelRoom> RetrieveUnoccupiedSingleBedHotelRooms(Hotel hotel)
        {
            using (var hotelDB = new SQLiteConnection(_connectionString))
            {
                // Find Hotel, then return a list  of its unoccupied single-bed HotelRooms.
                // return hotelDB.Table<Hotel>().ToList();
            }
        }


        // Retrieve a List of All Unoccupied Multi-bed Rooms at a Hotel.
        public static IList<HotelRoom> RetrieveUnoccupiedMultiBedHotelRooms(Hotel hotel)
        {
            using (var hotelDB = new SQLiteConnection(_connectionString))
            {
                // Find Hotel, then return a list  of its unoccupied multi-bed HotelRooms.
                // return hotelDB.Table<Hotel>().ToList();
            }
        }

    }
}
