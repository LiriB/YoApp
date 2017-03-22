﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YoApp.DataObjects.Users;
using YoApp.Data;

namespace YoApp.Identity.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FriendsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FriendsController(ILogger<FriendsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser([FromQuery]string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return BadRequest();

            var userInDb = await _unitOfWork.UserRepository.GetByNameAsync(phoneNumber);
            if (userInDb == null)
            {
                _logger.LogError($"User by {phoneNumber} requested by [{User.Identity.Name}] was not found.");
                return NotFound();
            }

            var dto = _mapper.Map<UserDto>(userInDb);
            
            return Ok(dto);
        }

        //Using POST Verb due to long querry object
        [HttpPost]
        public async Task<IActionResult> GetUsers([FromBody]IEnumerable<string> phoneNumbers)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var usersInDb = await _unitOfWork.UserRepository.GetByNamesAsync(phoneNumbers);
            if (!usersInDb.Any())
            {
                _logger.LogError($"No matching Users found for [{User.Identity.Name}] request.");
                return NotFound();
            }

            var matches = _mapper.Map<IEnumerable<UserDto>>(usersInDb);

            return Ok(matches);
        }

        [HttpGet("IsMember")]
        public async Task<IActionResult> IsMember(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return BadRequest();

            var result = await _unitOfWork.UserRepository.IsMemberAsync(phoneNumber);

            if (result)
                return Ok();
            else
                return NotFound();
        }
    }
}
