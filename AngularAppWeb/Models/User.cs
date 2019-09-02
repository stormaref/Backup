﻿using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AngularAppWeb.Models
{
    public class User
    {
        public static readonly List<User> Users = new List<User>();

        public string UserName { get; set; }
        public string ConnectionId { get; set; }
        [JsonIgnore]
        public Room CurrentRoom { get; set; }
        public Status Status { get; set; }

        public static void Remove(User user)
        {
            Users.Remove(user);
        }

        public static User Get(string connectionId)
        {
            return Users.SingleOrDefault(u => u.ConnectionId == connectionId);
        }

        public static User Get(string userName, string connectionId)
        {
            lock (Users)
            {
                User current = Users.SingleOrDefault(u => u.ConnectionId == connectionId);

                if (current == default(User))
                {
                    current = new User
                    {
                        UserName = userName,
                        ConnectionId = connectionId,
                        Status = Status.Available
                    };
                    Users.Add(current);
                }
                else
                {
                    current.UserName = userName;
                }

                return current;
            }
        }

        public static User ChangeConnectionId(string connectionId, User user)
        {
            Users.SingleOrDefault(u => u.UserName == user.UserName).ConnectionId = connectionId;
            return Users.SingleOrDefault(u => u.UserName == user.UserName);
        }
    }
    public enum Status { Busy, Available }
}
