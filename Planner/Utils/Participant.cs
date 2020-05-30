using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Utils
{
    public class Participant
    {
        public int Id { get; }
        public string Name { get; }
        public string Password { get; set; }

        public Participant(int ParticipantId, string ParticipantName)
        {
            this.Id = ParticipantId;
            this.Name = ParticipantName;
        }
    }
}
