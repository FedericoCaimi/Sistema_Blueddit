using SistemaBlueddit.Domain;
using System;
using System.Collections.Generic;

namespace SistemaBlueddit.Server.Logic
{
    public class UserLogic
    {
        private List<User> _users;

        public UserLogic()
        {
            _users = new List<User>();
        }

        public void CloseAll()
        {
            foreach (var user in _users)
            {
                user.TcpClient.GetStream().Close();
                user.TcpClient.Close();
            }
        }

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public void RemoveUser(User user)
        {
            _users.Remove(user);
        }

        public void ShowUsers()
        {
            foreach (var user in _users)
            {
                Console.WriteLine(user.PrintUser());
            }
        }
    }
}
