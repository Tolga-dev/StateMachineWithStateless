using System;

namespace Script.Core
{
    [Serializable]
    public class Command
    {
        public string id;

        public Command(string id)
        {
            this.id = id;
        }
        public Command()
        {
        }
    }
}