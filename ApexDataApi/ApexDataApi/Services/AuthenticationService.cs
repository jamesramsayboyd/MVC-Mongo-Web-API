namespace ApexDataApi.Services
{
    /// <summary>
    /// A service defining the username/password authentication function
    /// </summary>
    public class AuthenticationService
    {
        /// <summary>
        /// Username and password are set and checked here
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidateCredentials(string username, string password)
        {
            return username.Equals("admin") && password.Equals("admin");
        }
    }
}
