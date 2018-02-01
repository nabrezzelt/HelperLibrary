using System;
using HelperLibrary.PermissionManagement;

namespace HelperLibrary.ForumSystem
{
    public class LastTopic
    {
        public Topic Topic { get; set; }
        public int Posts { get; set; }
        public DateTime LastPostDateTime { get; set; }
        public IUser LastPostUser { get; set; }
    }
}