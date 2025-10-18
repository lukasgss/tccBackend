using Application.Common.Interfaces.Entities.UserMessages;
using Application.Queries.UserMessages.GetAllUsersConversations;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserMessageRepository : GenericRepository<UserMessage>, IUserMessageRepository
{
    private readonly AppDbContext _dbContext;

    public UserMessageRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserMessage?> GetByIdAsync(long messageId, Guid userId)
    {
        return await _dbContext.UserMessages
            .Include(message => message.Sender)
            .Include(message => message.Receiver)
            .SingleOrDefaultAsync(message => message.Id == messageId
                                             && !message.HasBeenDeleted
                                             && (message.Receiver.Id == userId
                                                 || message.Sender.Id == userId));
    }

    public async Task<int> GetNotificationsAsync(Guid userId)
    {
        return await _dbContext.UserMessages
            .Where(message => message.SenderId != userId && message.ReceiverId == userId && !message.HasBeenRead)
            .GroupBy(message => message.Sender)
            .CountAsync();
    }

    public async Task<IReadOnlyList<UserConversation>> GetAllUsersWithConversations(Guid userId)
    {
        return await _dbContext.UserMessages
            .Where(message => !message.HasBeenDeleted &&
                              (message.ReceiverId == userId || message.SenderId == userId))
            .GroupBy(message => message.SenderId == userId ? message.ReceiverId : message.SenderId)
            .Select(group => new UserConversation(
                group.Key,
                group.OrderByDescending(m => m.TimeStampUtc).First().SenderId == userId
                    ? group.OrderByDescending(m => m.TimeStampUtc).First().Receiver.Image
                    : group.OrderByDescending(m => m.TimeStampUtc).First().Sender.Image,
                group.OrderByDescending(m => m.TimeStampUtc).First().SenderId == userId
                    ? group.OrderByDescending(m => m.TimeStampUtc).First().Receiver.FullName
                    : group.OrderByDescending(m => m.TimeStampUtc).First().Sender.FullName,
                group.OrderByDescending(m => m.TimeStampUtc).First().Content,
                group.Count(m => m.ReceiverId == userId && !m.HasBeenRead),
                group.OrderByDescending(m => m.TimeStampUtc).First().TimeStampUtc
            ))
            .ToListAsync();
    }

    public async Task ReadAllAsync(Guid senderId, Guid receiverId)
    {
        await _dbContext.UserMessages
            .Where(message => message.Sender.Id == senderId
                              && message.Receiver.Id == receiverId
                              && !message.HasBeenDeleted).ExecuteUpdateAsync(message =>
                message.SetProperty(msg => msg.HasBeenRead, true));
    }
}