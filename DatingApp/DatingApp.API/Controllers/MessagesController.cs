
namespace DatingApp.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using AutoMapper;
    using DatingApp.API.Data;
    using DatingApp.API.Dtos;
    using DatingApp.API.Helpers;
    using DatingApp.API.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;


    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/users/{userId}/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDatingRepository _repo;

        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
            
        }    

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            // che the user token match the userId
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);

            if(messageFromRepo == null)
            return NotFound();

            return Ok(messageFromRepo);
        } 

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery] MessageParams messageParams)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            messageParams.UserId = userId;

            var messagesFromRepo = await _repo.GetMessagesForUSer(messageParams);

            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, 
            messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recepientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recepientId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var mesasgesFromRepo = await _repo.GetMessageThread(userId, recepientId);

            var messagesToReturn = _mapper.Map<IEnumerable<MessageToReturnDto>>(mesasgesFromRepo);

            return Ok(messagesToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId,[FromBody] MessageForCreationDto messageForCreationDto)
        {   
            var sender = await _repo.GetUser(userId);
            
            // sender.Id = userId
            if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            messageForCreationDto.SenderId = userId;

            var recepient = await _repo.GetUser(messageForCreationDto.RecepientId);

            if(recepient == null)
            return BadRequest("Could not find user");

            var message = _mapper.Map<Message>(messageForCreationDto);

            _repo.Add(message);

            if(await _repo.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);

                return CreatedAtRoute("GetMessage", new {id = message.Id}, messageToReturn);
            }
         
            throw new Exception("Creating the message failed on save");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMesege(int id, int userId)
        {
            if(userId!= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messagefromRepo = await _repo.GetMessage(id);

            if(messagefromRepo.SenderId == userId)
            messagefromRepo.SenderDeleded = true;

            if(messagefromRepo.RecepientId == userId)
            messagefromRepo.RecepientDeled = true;

            if(messagefromRepo.SenderDeleded == true && messagefromRepo.RecepientDeled == true)
            _repo.Delete(messagefromRepo);

            if(await _repo.SaveAll())
            return NoContent();

            throw new Exception("Error deleting the message");
        }
        
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            if(userId!= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messagefromRepo = await _repo.GetMessage(id);

            if(messagefromRepo.RecepientId != userId)
            return Unauthorized();

            messagefromRepo.IsRead = true;
            messagefromRepo.DateRead = DateTime.Now;

            await _repo.SaveAll();

            return NoContent();

        }
    }
}