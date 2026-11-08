using Ewallet.Api.DTOs;
using Ewallet.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ewallet.Api.Controllers;

[ApiController]
[Route("/api/users")]

public sealed class UserController : ControllerBase
{
    private readonly IUserServices _service;

    public UserController(IUserServices services)
    {
        _service = services;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IReadOnlyList<ProblemDetails>), StatusCodes.Status404NotFound)]
    
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _service.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var users = await _service.GetByIdAsync(id, cancellationToken);
        return users is null ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Data tidak ditemukan") : Ok(users);
    }


    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> Create([FromBody] UserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _service.CreateAsync(request, cancellationToken);
            if (created is null)
            {
                return Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "User gagal dibuat",
                    Detail = "Email atau WalletNumber mungkin sudah digunakan."
                });
            }

            return CreatedAtAction(
                nameof(GetById),
                new{id = created.Id},
                created
            );
        }
        catch (System.Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid create data",
                detail: e.Message
            );
        }
    }
}