using System.Threading.Tasks;
using Senparc.WebSocket.SignalR;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using Entity.Socket;
using NetCoreAPI.Socket;

namespace NetCoreAPI
{
    public class YvanHub : SenparcWebSocketHubBase
    {
        public static SocketHelp socket = new SocketHelp();
        /// <summary>
        /// 重写Hub连接事件
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            //查询用户
            var user = socket.Users.Where(w => w.UserName == Context.ConnectionId).FirstOrDefault();
            //判断用户是否存在
            if (user == null)
            {
                user = new User()
                {
                    UserName = Context.ConnectionId
                };
                socket.Users.Add(user);
            }
            //发送房间列表
            var rooms = socket.Rooms.Select(p => p.RoomName).ToList();
            //注册getRooms 获取房间的方法
            //Clients.Client(Context.ConnectionId).getRoomList(JsonConvert.SerializeObject(rooms));
            
            
            
            return base.OnConnectedAsync();
        }
        //更新所有用户的房间列表
        private void GetRooms()
        {
            var rooms = JsonConvert.SerializeObject(socket.Rooms.Select(p => p.RoomName).ToList());
            //Clients.All.getRoomList(rooms);
        }

        //重写Hub链接断开事件
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var user = socket.Users.Where(u => u.UserName == Context.ConnectionId).FirstOrDefault();
            //判断用户是否存在，存在则删除
            if (user != null)
            {
                //删除用户
                socket.Users.Remove(user);

            }
            return base.OnDisconnectedAsync(exception);
        }
        //加入聊天室
        public void AddRoom(string roomName)
        {
            //查询聊天室
            var room = socket.Rooms.Find(a => a.RoomName == roomName);
            //存在则加入
            if (room != null)
            {
                //查找房间中是否存在此用户
                var isUser = room.Users.Where(w => w.UserName == Context.ConnectionId).FirstOrDefault();
                //不存在则加入
                if (isUser == null)
                {
                    var user = socket.Users.Find(a => a.UserName == Context.ConnectionId);
                    user.Rooms.Add(room);
                    room.Users.Add(user);
                    Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                    //注册加入聊天室的addRoom方法
                    //Clients.Client(Context.ConnectionId).addRoom(roomName);
                }
                else
                {
                    //Clients.Client(Context.ConnectionId).showMessage("请勿重复加入房间");
                }
            }
        }
        //创建聊天室
        public void CreateRoom(string roomName)
        {
            var room = socket.Rooms.Find(a => a.RoomName == roomName);
            if (room == null)
            {
                Room r = new Room() { RoomName = roomName };
                //将房间加入列表
                socket.Rooms.Add(r);
                AddRoom(roomName);
                //Clients.Client(Context.ConnectionId).showMessage("房间创建完成");
                GetRooms();
            }
            else
            {
                //Clients.Client(Context.ConnectionId).showMessage("房间名重复");
            }
        }
        //退出聊天室
        public void ExitRoom(string roomName)
        {
            //查找房间是否存在
            var room = socket.Rooms.Find(a => a.RoomName == roomName);
            //存在则删除
            if (room != null)
            {
                //查找要删除的用户
                var user = room.Users.Where(p => p.UserName == Context.ConnectionId).FirstOrDefault();
                //移除此用户
                room.Users.Remove(user);
                //如果房间人数为0，怎删除房间
                if (room.Users.Count == 0)
                {
                    socket.Rooms.Remove(room);
                }
                //Groups Remove移除分组方法
                Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
                //提示客户端
                //Clients.Client(Context.ConnectionId).removeRoom("退出成功");
            }
        }
        //给分组内所有用户发送消息
        public void SendMsg(string Room, string Message)
        {
            SendAsync(Message, Clients.Group(Room), default);
        }
    }
    
}

