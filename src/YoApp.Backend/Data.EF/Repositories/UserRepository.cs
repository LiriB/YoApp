﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using YoApp.Backend.Data.Repositories;
using YoApp.Backend.Models;

namespace YoApp.Backend.Data.EF.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUser GetUser(string name)
        {
            return this.GetUserAsync(name).Result;
        }

        public async Task<ApplicationUser> GetUserAsync(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }

        public IEnumerable<ApplicationUser> GetUsers(IEnumerable<string> names)
        {
            return this.GetUsersAsync(names).Result;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync(IEnumerable<string> names)
        {
            var usersInDb = new List<ApplicationUser>();

            foreach (var phoneNumber in names)
            {
                var userInDb = await _userManager.FindByNameAsync(phoneNumber);
                if (userInDb != null)
                    usersInDb.Add(userInDb);
            }

            return usersInDb;
        }

        public IdentityResult AddUser(ApplicationUser user, string password)
        {
            return AddUserAsync(user, password).Result;
        }

        public async Task<IdentityResult> AddUserAsync(ApplicationUser user, string password)
        {
            if (user == null || string.IsNullOrWhiteSpace(password))
                return IdentityResult.Failed();

            return await _userManager.CreateAsync(user, password);
        }

        public void UpdateUserPassword(ApplicationUser user, string password)
        {
            UpdateUserPasswordAsync(user, password).RunSynchronously();
        }

        public async Task UpdateUserPasswordAsync(ApplicationUser user, string password)
        {
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, password);
        }
    }
}
