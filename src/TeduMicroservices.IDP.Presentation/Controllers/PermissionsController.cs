using System.Net;
using Microsoft.AspNetCore.Mvc;
using TeduMicroservices.IDP.Infrastructure.Repositories;
using TeduMicroservices.IDP.Infrastructure.ViewModels;

namespace TeduMicroservices.IDP.Presentation.Controllers;

[ApiController]
[Route("api/roles/{roleId}/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly IRepositoryManager _repository;
    
    public PermissionsController(IRepositoryManager repository)
    {
        _repository = repository;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PermissionViewModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetPermissions(string roleId)
    {
        var result = await _repository.Permission.GetPermissionsByRole(roleId);
        return Ok(result);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(PermissionViewModel), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreatePermission(string roleId, [FromBody] PermissionAddModel model)
    {
        var result = await _repository.Permission.CreatePermission(roleId, model);
        return result != null ? Ok(result) : NoContent();
    }
}