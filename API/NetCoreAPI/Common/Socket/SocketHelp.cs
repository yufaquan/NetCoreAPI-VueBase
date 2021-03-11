using Entity.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreAPI.Socket
{
    public class SocketHelp
    {
        public SocketHelp()
        {
            Users = new List<User>();
            Connections = new List<Connection>();
            Rooms = new List<Room>();
        }
        //用户集合
        public List<User> Users { get; set; }

        //连接集合
        public List<Connection> Connections { get; set; }

        //房间集合
        public List<Room> Rooms { get; set; }
    }
}
