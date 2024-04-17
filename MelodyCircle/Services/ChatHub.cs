using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace MelodyCircle.Services
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(Guid collaborationId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var userId = Context.UserIdentifier; 
            var user = await _context.Users.FindAsync(userId);
            var chatMessage = new ChatMessage
            {
                UserId = userId,
                CollaborationId = collaborationId,
                MessageText = message,
                Timestamp = DateTime.UtcNow
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            await Clients.Group(collaborationId.ToString()).SendAsync("ReceiveMessage", user.UserName, message, chatMessage.Timestamp.ToString("o"));
        }

        public override async Task OnConnectedAsync()
        {
            var collaborationId = Context.GetHttpContext().Request.Query["collaborationId"];
            if (!string.IsNullOrEmpty(collaborationId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, collaborationId);
            }
            await base.OnConnectedAsync();
        }

        public async Task LoadMessageHistory(Guid collaborationId)
        {
            var messages = await _context.ChatMessages
                                         .Where(m => m.CollaborationId == collaborationId)
                                         .OrderBy(m => m.Timestamp)
                                         .Select(m => new {
                                             m.User.UserName,
                                             m.MessageText,
                                             Timestamp = m.Timestamp.ToString("o")
                                         })
                                         .ToListAsync();

            await Clients.Caller.SendAsync("ReceiveMessageHistory", messages);
        }
    }
}
