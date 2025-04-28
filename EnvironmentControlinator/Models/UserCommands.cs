using System;
using System.Collections.Generic;
using System.Text;

namespace EnvironmentControlinator.Models
{
    public class UserCommands
    {
        public static int MAX_COLLOR_CHANGES_PER_USER = 30;

        public static int MAX_SOUNDS_PER_USER = 10;

        public UserCommands()
        {
            lastCollorChanged = lastSoundPlayed = DateTime.MinValue;
        }

        public int collorChanges { get; set; }

        public DateTime lastCollorChanged { get; set; }

        public int Sounds { get; set; }

        public DateTime lastSoundPlayed{ get; set; }
    }
}
