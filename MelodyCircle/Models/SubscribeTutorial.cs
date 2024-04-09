namespace MelodyCircle.Models
{
    public class SubscribeTutorial
    {
        public Guid Id { get; set; }

        public User User { get; set; }

        public Guid TutorialId { get; set; }

        public Tutorial Tutorial { get; set; }

        public List<Step> CompletedSteps { get; set; } = new List<Step>();
    }
}