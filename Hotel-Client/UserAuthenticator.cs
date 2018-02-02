namespace Hotel_Client
{
    /// <summary>
    /// This class is a fake authentication system with two hardcoded users. The first is an 
    /// admin account, intended for hotel employee use and the second is a general user account, 
    /// to be used by hotel customers. 
    /// 
    /// A hotel employee will login with her credentials and use the admin account to say, update 
    /// the database's tables. A user will login to the customer account and could check for hotels 
    /// with vacancies and see room information. The user could filter for the cheapest rooms, or 
    /// the most luxurious, or the closest hotel etc.
    /// </summary>
    public static class UserAuthenticator
    {
        // Declare usernames and passwords for the two accounts.
        private const string AdminAccount = "admin";
        private const string AdminPassword = "admin";

        private const string UserAccount = "user";
        private const string UserPassword = "user";

        // The User who is currently logged in.
        public static string LoggedInUser { get; private set; }


        // A Method that will attempt to log a user into the database.
        public static bool LoginToAccount(string enteredName, string enteredPassword)
        {
            bool loginAttemptStatus;

            // Test if the credentials match the Admin Account.
            if (enteredName == AdminAccount && enteredPassword == AdminPassword)
            {
                LoggedInUser = "Administrator";
                loginAttemptStatus = true;
            }
            // Test if the credentials match the Limited User Account.
            else if (enteredName == UserAccount && enteredPassword == UserPassword)
            {
                LoggedInUser = "User";
                loginAttemptStatus = true;
            }
            // User login failed.
            else
            {
                loginAttemptStatus = false;
            }

            return loginAttemptStatus;
        }
    }
}
