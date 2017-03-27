using Newtonsoft.Json;
using Rubito.SimpleFormsAuth;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using YoApp.Clients.Models;
using YoApp.DataObjects.Users;

namespace YoApp.Clients.Services
{
    /// <summary>
    /// Service to handle friends on the backend.
    /// </summary>
    public static class FriendsService
    {
        private static readonly Uri _baseAddress;
        private static readonly Uri _isMemberAddress;

        static FriendsService()
        {
            _baseAddress = new Uri(App.Settings.Friends.Url + "friends/");
            _isMemberAddress = new Uri(_baseAddress, "check");
        }

        /// <summary>
        /// Check if the there is a member registered under the given phonenumber.
        /// </summary>
        /// <param name="phoneNumber">Check by this.</param>
        /// <returns>Is this a member.</returns>
        public static async Task<bool> CheckMembership(string phoneNumber)
        {
            await AuthenticationService.RequestToken();

            var request = new OAuth2BearerRequest("GET",
                new Uri(_isMemberAddress, phoneNumber),
                null,
                AuthenticationService.AuthAccount);

            var response = await request.GetResponseAsync();

            return (response.StatusCode == HttpStatusCode.OK);
        }

        /// <summary>
        /// Check the range of given phonenumbers and returns only those who are members.
        /// </summary>
        /// <param name="phoneNumbers">Targets to check</param>
        /// <returns>Members</returns>
        public static async Task<IEnumerable<string>> CheckMembershipRange(IEnumerable<string> phoneNumbers)
        {
            if(phoneNumbers == null)
                throw new ArgumentNullException(nameof(phoneNumbers));

            await AuthenticationService.RequestToken();

            var request = new OAuth2BearerRequest("POST", 
                _isMemberAddress, 
                null, 
                AuthenticationService.AuthAccount);

            var serialized = JsonConvert.SerializeObject(phoneNumbers);
            request.SetRequestBody(serialized);

            var response = await request.GetResponseAsync();
            var text = response.GetResponseText();
            var deserialized = JsonConvert.DeserializeObject<IEnumerable<string>>(text);

            return deserialized;
        }

        /// <summary>
        /// Fetch a Friend from the backend by the given phone number.
        /// </summary>
        /// <param name="phoneNumber">A validated phoneNumber.</param>
        /// <returns>Friend from backend.</returns>
        public static async Task<Friend> FetchFriend(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            await AuthenticationService.RequestToken();

            var request = new OAuth2BearerRequest("GET",
                new Uri(_baseAddress, phoneNumber),
                null,
                AuthenticationService.AuthAccount);

            var response = await request.GetResponseAsync();
            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            var body = await response.GetResponseTextAsync();

            var dto = JsonConvert.DeserializeObject<UserDto>(body);
            return Friend.CreateFromDto(dto);
        }

        /// <summary>
        /// Fetch friends by the given phonenumbers. Non matching phoneNumbers will be ignored.
        /// </summary>
        /// <param name="phoneNumbers">Source phonenumbers.</param>
        /// <returns>Friends matching to given phoneNumbers.</returns>
        public static async Task<IEnumerable<Friend>> FetchFriends(IEnumerable<string> phoneNumbers)
        {
            if (phoneNumbers == null)
                throw new ArgumentNullException(nameof(phoneNumbers));

            await AuthenticationService.RequestToken();

            var request = new OAuth2BearerRequest("POST",
                _baseAddress,
                null,
                AuthenticationService.AuthAccount);

            var serialized = JsonConvert.SerializeObject(phoneNumbers);
            request.SetRequestBody(serialized);

            var response = await request.GetResponseAsync();
            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            var body = await response.GetResponseTextAsync();
            var dtos = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(body);

            var friends = new List<Friend>();
            foreach (var dto in dtos)
                friends.Add(Friend.CreateFromDto(dto));

            return friends;
        }

        /// <summary>
        /// Get nickname for the given user.
        /// </summary>
        /// <param name="phoneNumber">Target user to check by phoneNumber.</param>
        /// <returns>Users current nickname.</returns>
        public static async Task<string> GetName(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            await AuthenticationService.RequestToken();

            var request = new OAuth2BearerRequest("GET", 
                new Uri(_baseAddress, $"{phoneNumber}/name"),
                null,
                AuthenticationService.AuthAccount);

            var response = await request.GetResponseAsync();
            var body = await response.GetResponseTextAsync();

            return JsonConvert.DeserializeObject<string>(body);
        }

        /// <summary>
        /// Get status message for the given user.
        /// </summary>
        /// <param name="phoneNumber">Target user to check by phoneNumber.</param>
        /// <returns>Users current status message.</returns>
        public static async Task<string> GetStatus(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            await AuthenticationService.RequestToken();

            var request = new OAuth2BearerRequest("GET",
                new Uri(_baseAddress, $"{phoneNumber}/status"),
                null,
                AuthenticationService.AuthAccount);

            var response = await request.GetResponseAsync();
            var body = await response.GetResponseTextAsync();

            return JsonConvert.DeserializeObject<string>(body);
        }
    }
}