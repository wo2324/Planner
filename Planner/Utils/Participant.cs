﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Utils
{
    public class Participant
    {
        public int ParticipantId { get; }
        public string ParticipantName { get; }
        public string ParticipantPassword { get { return GetParticipantPassword(); } }

        public Participant(int ParticipantId, string ParticipantName)
        {
            this.ParticipantId = ParticipantId;
            this.ParticipantName = ParticipantName;
        }

        private string GetParticipantPassword()
        {
            return DbAdapter.ParticipantPasswordGet(this.ParticipantId);
        }
    }
}
