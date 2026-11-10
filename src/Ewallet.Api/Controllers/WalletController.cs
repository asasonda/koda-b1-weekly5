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


    
    [HttpPost("{usersId}/topups")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TopUp([FromRoute] int usersId, [FromBody] TopUpRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.TopUpAsync(usersId, request, cancellationToken);
            if (result is null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Wallet tidak ditemukan",
                    detail: $"Wallet untuk user ID {usersId} tidak ditemukan."
                );
            }

            return Ok(new
            {
                message = "Top up berhasil",
                data = result
            });
        }
        catch (ArgumentException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Top up gagal",
                detail: ex.Message
            );
        }
        catch (Exception)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error",
                detail: "Terjadi kesalahan saat memproses top up."
            );
        }
    }

}