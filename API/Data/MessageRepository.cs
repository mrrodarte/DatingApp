using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

         public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

         public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                        .OrderByDescending(x => x.MessageSent)
                        .AsQueryable();

            query = messageParams.Container switch{
                "Inbox" => query.Where(u => u.RecepientUserName == messageParams.UserName
                    && u.RecepientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUserName == messageParams.UserName
                    && u.SenderDeleted == false),
                _ => query.Where(u => u.RecepientUserName == messageParams.UserName 
                    && u.DateRead == null && u.RecepientDeleted == false)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages,messageParams.PageNumber,
                messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, 
            string RecepientUserName)
        {
            var query = _context.Messages
                .Where(
                    m => m.RecepientUserName == currentUserName && m.RecepientDeleted == false
                    && m.SenderUserName == RecepientUserName
                    || m.RecepientUserName == RecepientUserName && m.SenderDeleted == false
                    && m.SenderUserName == currentUserName)
                .OrderBy(m => m.MessageSent)
                .AsQueryable();
                
            var unreadMessages = query
                .Where(m => m.DateRead == null && m.RecepientUserName == currentUserName).ToList();

            if (unreadMessages.Any())
            {
                foreach(var message in unreadMessages){
                    message.DateRead = DateTime.UtcNow;
                }
            };

            return await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

    }
}