﻿using YoApp.Data;
using YoApp.Data.Repositories;

namespace YoApp.Friends.Core
{
    public class Persistence : UnitOfWorkBase, IFriendsPersistence
    {
        public IFriendsRepository Friends { get; }

        public Persistence(ApplicationDbContext context, IFriendsRepository friendsRepository) : base(context)
        {
            Friends = friendsRepository;
        }
    }
}
