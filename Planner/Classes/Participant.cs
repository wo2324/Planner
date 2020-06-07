namespace Planner.Classes
{
    public class Participant
    {
        public string Name { get; }
        public string Password { get; set; }

        public Participant(string participantName, string participantPassword)
        {
            this.Name = participantName;
            this.Password = participantPassword;
        }
    }
}
