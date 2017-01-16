namespace AuthorizationApp
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }

        public User() { }
        
        public User(string login, string password, string name)
        {
            Login = login;
            Password = password;
            Name = name;
        }
    }
}
