using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class ChatHub : Hub
{
    private static readonly Dictionary<int, string> _userConnections = new();
    private readonly AppDbContext _dbContext;

    public ChatHub(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task OnConnectedAsync()
    {
        var userIdClaim = Context.User?.FindFirst("userId");
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            _userConnections[userId] = Context.ConnectionId;

            // Обновить LastSeen пользователя
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                user.LastSeen = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            await Clients.All.SendAsync("UserOnline", userId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdClaim = Context.User?.FindFirst("userId");
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            _userConnections.Remove(userId);

            // Обновить LastSeen
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                user.LastSeen = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            await Clients.All.SendAsync("UserOffline", userId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(int receiverId, string content)
    {
        var senderId = int.Parse(Context.User!.FindFirst("userId")!.Value);

        // Сохранить сообщение в БД
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync();

        // Отправить получателю если онлайн
        if (_userConnections.TryGetValue(receiverId, out string? connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }

        // Подтверждение отправителю
        await Clients.Caller.SendAsync("MessageSent", message);
    }

    public async Task MarkAsRead(int messageId)
    {
        var message = await _dbContext.Messages.FindAsync(messageId);
        if (message != null && message.ReceiverId == GetCurrentUserId())
        {
            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            // Уведомить отправителя
            if (_userConnections.TryGetValue(message.SenderId, out string? connectionId))
            {
                await Clients.Client(connectionId).SendAsync("MessageRead", messageId);
            }
        }
    }

    public async Task TypingIndicator(int receiverId, bool isTyping)
    {
        if (_userConnections.TryGetValue(receiverId, out string? connectionId))
        {
            var senderId = GetCurrentUserId();
            await Clients.Client(connectionId).SendAsync("UserTyping", senderId, isTyping);
        }
    }

    private int GetCurrentUserId()
    {
        return int.Parse(Context.User!.FindFirst("userId")!.Value);
    }

    // Проверка онлайн-статуса пользователя
    public static bool IsUserOnline(int userId) => _userConnections.ContainsKey(userId);
}