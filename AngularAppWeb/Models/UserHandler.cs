using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;

namespace AngularAppWeb.Models
{
    public class UserHandler
    {
        public static readonly List<UserHandler> All = new List<UserHandler>();
        public HubConnection Connection { get; set; }
        public User User { get; set; }
        public static void Add(UserHandler user)
        {
            All.Add(user);
        }
    }
}
