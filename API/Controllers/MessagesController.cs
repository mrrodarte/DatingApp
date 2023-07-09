using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public MessagesController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();

            if (username == createMessageDto.RecepientUserName.ToLower())
                return BadRequest("You cannot send messages to yourself");

            var sender = await _uow.UserRepository.GetUserByUserNameAsync(username);
            var recepient = await _uow.UserRepository.GetUserByUserNameAsync(createMessageDto.RecepientUserName);

            if (recepient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recepient = recepient,
                SenderUserName = sender.UserName,
                RecepientUserName = recepient.UserName,
                Content = createMessageDto.Content
            };

            _uow.MessageRepository.AddMessage(message);

            if (await _uow.Complete()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]
            MessageParams messageParams)
        {
            messageParams.UserName = User.GetUserName();

            var messages = await _uow.MessageRepository.GetMessageForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage,
                messages.PageSize,messages.TotalCount,messages.TotalPages));

            return messages;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUserName();

            var message = await _uow.MessageRepository.GetMessage(id);

            if (message.SenderUserName != username && message.RecepientUserName != username) 
                return Unauthorized();

            if (message.SenderUserName == username) message.SenderDeleted = true;
            if (message.RecepientUserName == username) message.RecepientDeleted = true;

            if (message.SenderDeleted && message.RecepientDeleted) 
            {
                _uow.MessageRepository.DeleteMessage(message);
            }

            if (await _uow.Complete()) return Ok();

            return BadRequest("Problem deleting the message");

        }
    }
}