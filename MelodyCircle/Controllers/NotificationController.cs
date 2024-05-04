﻿using MelodyCircle.Services;
using Microsoft.AspNetCore.Mvc;

namespace MelodyCircle.Controllers
{
    public class NotificationController : Controller
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /* Guilherme Bernardino */
        public async Task<IActionResult> Index()
        {
            var userId = User.Identity.Name;
            var notifications = await _notificationService.GetNotificationsAsync(userId);

            return View(notifications);
        }

        /* Guilherme Bernardino */
        [HttpPost]
        public async Task<IActionResult> AcceptInvite(Guid notificationId)
        {
            await _notificationService.AcceptCollaborationInviteAsync(notificationId);
            return RedirectToAction("Index", "Home"); 
        }

        /* Guilherme Bernardino */
        [HttpPost]
        public async Task<IActionResult> DeclineInvite(Guid notificationId)
        {
            await _notificationService.DeclineCollaborationInviteAsync(notificationId);
            return RedirectToAction("Index", "Home");
        }
    }
}
