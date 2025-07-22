using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Demo.DAL.Entities.Identity;

public class UserActivityFilter : IAsyncActionFilter
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserActivityFilter(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = await _userManager.GetUserAsync(context.HttpContext.User);
        if (user != null)
        {
            user.LastActivity = DateTime.Now;
            await _userManager.UpdateAsync(user);
        }

        await next(); // Continua con l'esecuzione dell'azione
    }
}
