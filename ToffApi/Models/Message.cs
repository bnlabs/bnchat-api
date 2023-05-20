using System;

namespace Models;

public class Message
{
    public Message()
    {

    }

    public Guid id { get; set; }

    public Guid receiverId { get; set; }

    public Guid senderId { get; set; }

    public string content { get; set; }

    public DateTime timeStamp { get; set; }

}