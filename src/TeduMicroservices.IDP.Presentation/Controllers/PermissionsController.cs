using Microsoft.AspNetCore.Mvc;
using TeduMicroservices.IDP.Infrastructure.Repositories;

namespace TeduMicroservices.IDP.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly IRepositoryManager _repository;
    
    public PermissionsController(IRepositoryManager repository)
    {
        _repository = repository;
    }
}