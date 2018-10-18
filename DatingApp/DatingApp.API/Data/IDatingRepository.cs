using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T: class;

        void Delete<T>(T entity) where T: class;

        Task<bool> SaveAll();

        // Task<IEnumerable<User>> GetsUsers();
        Task<PagedList<User>> GetUsers(UserParams userParams);

        Task<User> GetUser(int id);

        Task<Photo> GetPhoto(int id);

        Task<Like> GetLike(int userId, int recepientId);

        Task<Photo> GetMainPhotoForUser(int userId);

        Task<Message> GetMessage(int id);

        Task<PagedList<Message>> GetMessagesForUSer(MessageParams messageParams);

        // conversation between users in tab module
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recepientId);
    }
}