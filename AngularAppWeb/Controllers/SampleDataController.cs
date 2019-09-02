using AngularAppWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AngularAppWeb.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private static readonly string url = "http://localhost:10580/sgr/rtc";

        [HttpGet("[action]")]
        public async Task<IActionResult> GetConnectionId([FromQuery]string userName)
        {
            UserHandler user = FindUser(userName);
            if (user == default(UserHandler))
                return NotFound("Couldn't find user");
            try
            {
                await user.Connection.StartAsync();
                var str = await user.Connection.InvokeAsync<string>("GetConnectionId");
                return Ok(str);
            }
            catch (Exception)
            {
                return BadRequest("Connection is not established");
            }
        }

        private UserHandler FindUser(string userName)
        {
            return UserHandler.All.SingleOrDefault(u => u.User.UserName == userName);
        }

        /// <summary>
        /// Joining a client to a specific room
        /// </summary>
        /// <param name="userName">Join in hub with this username</param>
        /// <param name="roomName">Which room you wanna join (room must be available)</param>
        /// <returns>Http status code</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> JoinRoom(string userName, string roomName)
        {
            if (!CheckUsername(userName))
                return NotFound("User is not available");
            if (CheckRoom(roomName))
                return NotFound("Room is not available");
            var connection = new HubConnectionBuilder().WithUrl(url).Build();
            await connection.StartAsync();
            var user = await connection.InvokeAsync<User>("Join", userName, roomName);
            await GetUserHandler(user, connection);
            return Ok(user);
        }

        /// <summary>
        /// Join caller client to hub
        /// </summary>
        /// <param name="userName">Join in hub with this username</param>
        /// <returns>Http status code</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> Join(string userName)
        {
            if (!CheckUsername(userName))
                return BadRequest("Username is not available");
            var room = Guid.NewGuid().ToString();
            var connection = new HubConnectionBuilder().WithUrl(url).Build();
            await connection.StartAsync();
            var user = await connection.InvokeAsync<User>("Join", userName, room);
            await GetUserHandler(user, connection);
            return Ok(user);
        }

        private async static Task GetUserHandler(User user, HubConnection connection)
        {
            var connectionId = await connection.InvokeAsync<string>("GetConnectionId");
            user = Models.User.ChangeConnectionId(connectionId, user);
            if (user.ConnectionId != connectionId)
            {
                throw new NotAvailableException("Didn't change correctly");
            }
            UserHandler.Add(new UserHandler
            {
                User = user,
                Connection = connection
            });
        }

        [HttpGet("[action]")]
        public bool CheckRoom(string roomName)
        {
            lock (Room.Rooms)
            {
                var current = Room.Rooms.SingleOrDefault(r => r.Name == roomName);
                return current == null;
            }
        }

        [HttpGet("[action]")]
        public IActionResult GetRoomName(string userName)
        {
            var user = FindUser(userName);
            if (user == default(UserHandler))
                return NotFound("User not found");
            return Ok(user.User.CurrentRoom.Name);
        }

        /// <summary>
        /// Simple function to check username duplicating
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private bool CheckUsername(string userName)
        {
            User user = Models.User.Users.SingleOrDefault(u => u.UserName.Equals(userName));
            return user == null;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> MakeCall(string userName, string signal, string targetConnectionId)
        {
            var user = FindUser(userName);
            if (user == null)
                return NotFound();
            await user.Connection.StartAsync();
            await user.Connection.InvokeAsync("SendSignal", signal, targetConnectionId);
            return Ok();
        }

        /// <summary>
        /// Hangup call
        /// </summary>
        /// <returns>Https status code</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> Hangup(string userName)
        {
            var user = FindUser(userName);
            if (user == null)
                return NotFound();
            return await user.Connection.InvokeAsync<IActionResult>("Hangup");
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetIceServers(string username, string credential)
        {
            return Ok(new RtcIceServer[] { new RtcIceServer() { Username = username, Credential = credential } });
        }
    }
}
