using Ewallet.Api.DTOs;
using Ewallet.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ewallet.Api.Controllers;

[ApiController]
[Route("api/transfers")]
public sealed class TransferController : ControllerBase
{
    private readonly IWalletServices _service;

    public TransferController(IWalletServices service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.TransferAsync(request, cancellationToken);
            return Ok(new
            {
                message = "Transfer berhasil",
                data = result
            });
        }
        catch (ArgumentException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Transfer gagal",
                detail: ex.Message
            );
        }
        catch (KeyNotFoundException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Wallet tidak ditemukan",
                detail: ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Transfer gagal",
                detail: ex.Message);
        }
    }
}