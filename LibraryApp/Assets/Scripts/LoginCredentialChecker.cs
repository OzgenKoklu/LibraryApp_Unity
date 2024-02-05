using System.Linq;
public static class LoginCredentialChecker
{
    //This class is going to serve as a login credential checker, ATM only one user is will be able to login so no need to have a system to store and retreive user data just now.
    private const string AdminUsername = "admin";
    private const string AdminPassword = "1234";


    public static bool CheckCredentials(string enteredUsername, string enteredPassword)
    {
        // a database or some secure storage for real credentials are essential
        // For simplicity, we are using hardcoded values here

        // Check if the entered credentials match the expected admin credentials
        return enteredUsername == AdminUsername && enteredPassword == AdminPassword;
    }

}
