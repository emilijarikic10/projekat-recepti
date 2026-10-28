using Recepti.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Recepti.Services
{
    public class UserService
    {
        private readonly string UserFile =
          Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "users.xml");

        public List<User> LoadUsers()
        {
            if (!File.Exists(UserFile))
                return new List<User>();

            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));

            using (FileStream fs = new FileStream(UserFile, FileMode.Open))
            {
                return (List<User>)serializer.Deserialize(fs);
            }
        }

        public User ValidateUser(string username, string password)
        {
            var users = LoadUsers();

            if (users == null || users.Count == 0)
                return null;

            return users.FirstOrDefault(u =>u.Username == username && u.Password == password);
        }

        public void SaveUsers(List<User> users)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));

            using (FileStream fs = new FileStream(UserFile, FileMode.Create))
            {
                serializer.Serialize(fs, users);
            }
        }
    }
}