using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MelodyCircle.Services
{
    /* Guilherme Bernardino */
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public NotificationService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                Status = NotificationStatus.Pending,
                CreatedDate = DateTime.Now
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task AcceptCollaborationInviteAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.Status = NotificationStatus.Accepted;


                var collaboration = await _context.Collaborations
                    .Include(c => c.ContributingUsers)
                    .Include(c => c.WaitingUsers)
                    .FirstOrDefaultAsync(c => c.Id == notification.CollaborationId);

                if (collaboration != null)
                {
                    var user = await _userManager.FindByNameAsync(notification.RecipientId);

                    collaboration.WaitingUsers.Remove(user);
                    collaboration.ContributingUsers.Add(user);
                    await _context.SaveChangesAsync();
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeclineCollaborationInviteAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.Status = NotificationStatus.Declined;

                var collaboration = await _context.Collaborations
                  .Include(c => c.WaitingUsers)
                  .FirstOrDefaultAsync(c => c.Id == notification.CollaborationId);

                if (collaboration != null)
                {
                    var user = await _userManager.FindByNameAsync(notification.RecipientId);


                    collaboration.WaitingUsers.Remove(user);
                    await _context.SaveChangesAsync();
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveOldNotificationsAsync()
        {
            var pendingNotificationsToRemove = await _context.Notifications
                .Where(n => n.Status == NotificationStatus.Pending && n.CreatedDate < DateTime.UtcNow.AddDays(-10))
                .ToListAsync();

            _context.Notifications.RemoveRange(pendingNotificationsToRemove);

            var otherNotificationsToRemove = await _context.Notifications
                .Where(n => n.Status != NotificationStatus.Pending && n.CreatedDate < DateTime.UtcNow.AddDays(-1))
                .ToListAsync();

            _context.Notifications.RemoveRange(otherNotificationsToRemove);

            await _context.SaveChangesAsync();
        }
    }
}
