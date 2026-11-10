using Ewallet.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Ewallet.Api.DTOs;
using Ewallet.Api.ViewModels;

namespace Ewallet.Api.ViewControllers;

[Route("wallet")]
public sealed class WalletPageController : Controller
{
    private readonly IWalletServices _walletServices;

    public WalletPageController(IWalletServices walletServices)
    {
        _walletServices = walletServices;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detail(
        int id,
        CancellationToken cancellationToken)
    {
        var wallet = await _walletServices.GetByIdUsersAsync(
            id,
            cancellationToken
        );

        if (wallet is null)
            return NotFound();

        return View(wallet);
    }

    [HttpGet("{id:int}/topup")]
    public async Task<IActionResult> TopUp(
        int id,
        CancellationToken cancellationToken)
    {
        var wallet = await _walletServices.GetByIdUsersAsync(
            id,
            cancellationToken
        );

        if (wallet is null)
            return NotFound();

        return View(new TopUpViewModel());
    }

    [HttpPost("{id:int}/topup")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TopUp(
        int id,
        TopUpViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var request = new TopUpRequest(model.Amount);

            var result = await _walletServices.TopUpAsync(
                id,
                request,
                cancellationToken
            );

            if (result is null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Top Up gagal diproses."
                );

                return View(model);
            }

            TempData["Success"] = "Top Up berhasil dilakukan.";

            return RedirectToAction(nameof(Detail), new { id });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}