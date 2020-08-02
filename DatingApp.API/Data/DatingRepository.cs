using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
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

        public async Task<Like> GetLike(int userId, int recepientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(user =>
                user.LikerId == userId && user.LikeeId == recepientId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(user => user.UserId == userId)
                .FirstOrDefaultAsync(photo => photo.IsMain);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(message => message.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages
                .Include(message => message.Sender).ThenInclude(sender => sender.Photos)
                .Include(message => message.Recipient).ThenInclude(recipient => recipient.Photos)
                .AsQueryable();

            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(message => message.RecipientId == messageParams.UserId 
                        && message.RecipientDeleted == false);
                    break;
                case "Outbox":
                    messages = messages.Where(message => message.SenderId == messageParams.UserId 
                        && message.SenderDeleted == false);
                    break;
                default:
                    messages = messages.Where(message => message.RecipientId == messageParams.UserId 
                        && message.RecipientDeleted == false && message.IsRead == false);
                    break;
            }

            messages = messages.OrderByDescending(message => message.DateSent);

            return await PagedList<Message>.CreateAsync(messages,
                messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages
                .Include(message => message.Sender).ThenInclude(sender => sender.Photos)
                .Include(message => message.Recipient).ThenInclude(recipient => recipient.Photos)
                .Where(message => message.RecipientId == userId && message.RecipientDeleted == false 
                    && message.SenderId == recipientId
                    || message.RecipientId == recipientId && message.SenderId == userId 
                    && message.SenderDeleted == false)
                .OrderByDescending(message => message.DateSent)
                .ToListAsync();

            return messages;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(u => u.Photos)
                .OrderByDescending(user => user.LastActive).AsQueryable();

            users = users.Where(user => user.Id != userParams.UserId);

            users = users.Where(user => user.Gender == userParams.Gender);

            if (userParams.Likers)
            {
                var receivedLikes = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => receivedLikes.Contains(u.Id));
            }

            if (userParams.Likees)
            {
                var sentLikes = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => sentLikes.Contains(u.Id));
            }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDateOfBirth = DateTime.Today.AddYears(-userParams.MaxAge);
                var maxDateOfBirth = DateTime.Today.AddYears(-userParams.MinAge + 1);

                users = users.Where(user => user.DateOfBirth >= minDateOfBirth && user.DateOfBirth <= maxDateOfBirth);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(user => user.Created);
                        break;
                    default:
                        users = users.OrderByDescending(user => user.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        private async Task<IEnumerable<int>> GetUserLikes(int userId, bool likers)
        {
            var user = await _context.Users
                .Include(u => u.ReceivedLikes)
                .Include(u => u.SentLikes)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (likers)
            {
                return user.ReceivedLikes
                    .Where(u => u.LikeeId == userId)
                    .Select(like => like.LikerId);
            }
            else
            {
                return user.SentLikes
                    .Where(u => u.LikerId == userId)
                    .Select(like => like.LikeeId);
            }
        }
    }
}