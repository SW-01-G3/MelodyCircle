﻿namespace MelodyCircle.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public Guid CollaborationId { get; set; }
        public string CollaborationTitle { get; set; }
        public string CollaborationDescription { get; set; }
        public NotificationStatus Status { get; set; }
    }
    public enum NotificationStatus
    {
        Pending,
        Accepted,
        Declined
    }
}
