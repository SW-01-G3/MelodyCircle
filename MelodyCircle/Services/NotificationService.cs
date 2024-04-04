using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.EntityFrameworkCore;

namespace MelodyCircle.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetNotificationsAsync(string userId)
        {
            return await _context.Notifications.Where(n => n.RecipientId == userId).ToListAsync();
        }

        public async Task SendCollaborationInviteAsync(string senderId, string recipientId, Guid collaborationId, string collaborationTitle, string collaborationDescription)
        {
            var notification = new Notification
            {
                SenderId = senderId,
                RecipientId = recipientId,
                CollaborationId = collaborationId, 
                CollaborationTitle = collaborationTitle,
                CollaborationDescription = collaborationDescription,
                Status = NotificationStatus.Pending
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task AcceptCollaborationInviteAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.Status = NotificationStatus.Accepted;
                var collaboration = await _context.Collaborations.FindAsync(notification.CollaborationId);
                if (collaboration != null)
                {
                    var user = await _context.Users.FindAsync(notification.RecipientId);

                    collaboration.WaitingUsers.Remove(user);
                    collaboration.ContributingUsers.Add(user);
                    await _context.SaveChangesAsync();
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeclineCollaborationInviteAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.Status = NotificationStatus.Declined;
                var collaboration = await _context.Collaborations.FindAsync(notification.CollaborationId);
                if (collaboration != null)
                {
                    var user = await _context.Users.FindAsync(notification.RecipientId);

                    collaboration.WaitingUsers.Remove(user);
                    await _context.SaveChangesAsync();
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
