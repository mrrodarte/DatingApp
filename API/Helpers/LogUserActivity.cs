using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next(); //next is for after request executed.  for before use context.

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userid = resultContext.HttpContext.User.GetUserId();

            var uow = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();

            //set last active on the user and save changes
            var user = await uow.UserRepository.GetUserByIdAsync(userid);
            user.LastActive = DateTime.UtcNow;
            await uow.Complete();
        }
    }
}