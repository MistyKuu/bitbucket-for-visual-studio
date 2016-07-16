namespace GitClientVS.Contracts.Models
{
    public class ConnectionData
    {
        private ConnectionData(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        private ConnectionData()
        {
            IsLoggedIn = false;
        }

        public static ConnectionData NotLogged => new ConnectionData();

        public static ConnectionData Create(string userName, string password)
        {
            return new ConnectionData(userName, password);
        }

        public bool IsLoggedIn { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
    }
}