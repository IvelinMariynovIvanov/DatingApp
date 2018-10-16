using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DataRepository(DataContext context)
        {
            _context = context;

        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
           return await _context.Photos.Where(u => u.UserId == userId).
                                        FirstOrDefaultAsync(p =>p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        // public async Task<IEnumerable> GetsUsers()
        // {
        //     var users = await _context.Users.Include(p => p.Photos).ToListAsync();

        //     return users;
        // }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users.Include(p => p.Photos)
            .OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(u => u.Id != userParams.UserId);

            users = users.Where(u => u.Gender == userParams.Gender);

            if(userParams.Likers)
            {
                var usersLikers = await GetUserLikes(userParams.UserId, userParams.Likers);

                users = users.Where(u => usersLikers.Contains(u.Id));
            }

            if(userParams.Likees)
            {
                var usersLikees = await GetUserLikes(userParams.UserId, userParams.Likers);

                users = users.Where(u => usersLikees.Contains(u.Id));
            }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDateOfbirth = DateTime.Today.AddYears(-userParams.MaxAge -1 );
                var maxDateOfBirth = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDateOfbirth && u.DateOfBirth <= maxDateOfBirth);
            }

          if(!string.IsNullOrEmpty(userParams.OrderBy))
          {
            switch (userParams.OrderBy)
            {
                case "created": 
                    users = users.OrderByDescending(u => u.Created);
                    break;
                default: 
                    users = users.OrderByDescending(u => u.LastActive);
                    break;
            }
          }

            return  await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            // id current logged user id

            var user = await _context.Users
            .Include(l => l.Likers)
            .Include(l => l.Likees)
            .FirstOrDefaultAsync(u => u.Id == id);

            if(likers)
            {
                // get user who  looged user liked
                return  user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return  user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }


        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<bool> SaveAll()
        {
            bool isDbChanged = await _context.SaveChangesAsync() > 0;

            return isDbChanged;
        }

        public async Task<Like> GetLike(int userId, int recepientId)
        {
            // userId -> likerId
            // recepientId -> likeeId

            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recepientId);

        }
    }
}