using Ewallet.Api.DTOs;
using Ewallet.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ewallet.Api.Controllers;

[ApiController]
[Route("/api/wallets")]
public sealed class WalletController : ControllerBase
{
    private readonly IWalletServices _service;

    public WalletController(IWalletServices services)
    {
        _service = services;
    }

    [HttpGet("{usersId}")]
    [ProducesResponseType(typeof(WalletResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

    public async Task<ActionResult<IReadOnlyList<WalletResponse>>> GetByUsersId([FromRoute] int usersId, CancellationToken cancellationToken)
    {
        var wallets = await _service.GetByIdUsersAsync(usersId, cancellationToken);
        return wallets is null ? Problem(statusCode: StatusCodes.Status404NotFound, title:"Data tidak ditemukan") : Ok(wallets);
    }
}