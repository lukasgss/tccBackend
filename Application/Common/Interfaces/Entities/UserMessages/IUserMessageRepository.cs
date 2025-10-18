using Application.Common.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Common.Interfaces.Entities.UserMessages;

public interface IUserMessageRepository : IGenericRepository<UserMessage>
{
    Task<UserMessage?> GetByIdAsync(long messageId, Guid userId);

    Task ReadAllAsync(Guid senderId, Guid receiverId);
}