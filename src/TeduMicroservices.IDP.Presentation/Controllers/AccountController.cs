using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TeduMicroservices.IDP.Infrastructure.Entities;

namespace TeduMicroservices.IDP.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize("Bearer")]
public class AccountController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public AccountController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> UserInfo()
    {
        var user = await _userManager.FindByNameAsync(User.Identity!.Name);
        return Ok(user);
    }
}