using Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Request
{
 

    public class SendNotiSignalr
    {
        public List<UserToNotification> UserTo { get; set; }
        public QueueServiceBusiness DataMessage { get; set; }
    }

    public class UserToNotification
    {
        public string Id { get; set; }
        public int Level { get; set; }
        public string Title { get; set; }
        public string Business_Type_Step { get; set; }
        public string MessageContent { get; set; }
        public object DataInfo { get; set; }
    }
}
