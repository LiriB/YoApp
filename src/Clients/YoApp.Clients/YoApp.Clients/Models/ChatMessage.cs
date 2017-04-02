﻿using Realms;
using System;

namespace YoApp.Clients.Models
{
    public class ChatMessage : RealmObject
    {
        public enum Delivery
        {
            Pending,
            Send,
            Received
        }

        [PrimaryKey]
        public string Id { get; private set; }

        public bool IsIncomming { get; set; }
        public int DeliveryState { get; set; }
        public string Message { get; set; }
        public DateTimeOffset Date { get; private set; }

        public ChatMessage()
        {
            Id = Guid.NewGuid().ToString();
            Date = DateTimeOffset.UtcNow;
        }
    }
}
