﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entity.Socket
{
    //用户类
    public class User
    {
        [Key]
        public string UserName { get; set; }
        //用户连接
        public List<Connection> Connections { get; set; }
        //用户房间集合
        public virtual List<Room> Rooms { get; set; }
        public User()
        {
            Connections = new List<Connection>();
            Rooms = new List<Room>();
        }
    }
    public class Connection
    {
        //连接ID
        public string ConnectionID { get; set; }
        //用户代理
        public string userAgent { get; set; }
        //是否连接
        public bool Connected { get; set; }
    }
    //房间类
    public class Room
    {
        [Key]
        public string RoomName { get; set; }
        //用户集合
        public virtual List<User> Users { get; set; }
        public Room()
        {
            Users = new List<User>();
        }
    }
}
