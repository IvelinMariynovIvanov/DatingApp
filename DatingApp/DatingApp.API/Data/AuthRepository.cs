
namespace DatingApp.API.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DatingApp.API.Models;
    using Microsoft.EntityFrameworkCore;
    using DatingApp.API;
    using System.Text;

    public class AuthRepository : IAuthRepository
    {
        private readonly Datacontext _context;
        public AuthRepository(Datacontext context)
        {
            this._context = context;
        }

        public async Task<bool> Exist(string username)
        {
            if(await _context.Users.AnyAsync(u => u.UserName == username))
            return true;

            return false;
        }

        public async Task<User> Login(string username, string password)
        {
            
           var user = await _context.Users.FirstOrDefaultAsync(u =>u.UserName == username);
           //var t = user.GeneratePasswordHash;

           if(user == null)
           return null;

           bool verifyPasswordHash = VerifyPasswordHash(password, user.PasswordHash, user.PasswodSalt);

           if(verifyPasswordHash == false)
           return null;

           return user; 

        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
           using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
           {
               var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

               for (int i = 0; i < computedHash.Length; i++)
               {
                   if(computedHash[i] != passwordHash[i])
                   {
                       return false;
                   }
               }
               return true;
           }
           //return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;

            GeneratePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswodSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void GeneratePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
           using (var hmac = new System.Security.Cryptography.HMACSHA512())
           {
               //passwordHash = hmac.Key;
              // passwordSalt = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
           }
        }
    }
}