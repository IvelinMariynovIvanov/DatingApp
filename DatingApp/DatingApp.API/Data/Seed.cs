using System.Collections.Generic;
using System.Text;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly Datacontext _context;

        public Seed(Datacontext context)
        {
            _context = context;

        }

        public void SeedUsers()
        {
            var usersData = System.IO.File.ReadAllText("Data/UserSeedData.json");

            var users = JsonConvert.DeserializeObject<List<User>>(usersData);

            foreach (var user in users)
            {
                byte[] passwordHash, passwordSalt;
                // all users has same password - "password"
                GeneratePasswordHash("password", out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswodSalt = passwordSalt;
                user.UserName = user.UserName.ToLower();

                _context.Users.Add(user);
            }

            _context.SaveChanges();   
        }

         private void GeneratePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
           using (var hmac = new System.Security.Cryptography.HMACSHA512())
           {  
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
           }
        }
    }
}