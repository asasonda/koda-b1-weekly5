using Ewallet.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Ewallet.Api.DTOs;
using Ewallet.Api.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ewallet.Api.ViewControllers;

[Route("wallet")]
public sealed class WalletPageController : Controller
{
    private readonly IWalletServices _walletServices;
    private readonly IUserServices _userServices;

    public WalletPageController(IWalletServices walletServices, IUserServices userServices)
    {
        _walletServices = walletServices;
        _userServices = userServices;
    }

    private async Task LoadWalletOptionsAsync(
        int senderUsersId,
        TransferViewModel model,
        CancellationToken cancellationToken)
    {
        var users = await _userServices.GetAllAsync(cancellationToken);

        foreach (var user in users)
        {
            if (user.Id == senderUsersId)
                continue;

            var wallet = await _walletServices.GetByIdUsersAsync(
                user.Id,
                cancellationToken);

            if (wallet is null)
                continue;

            model.WalletOptions.Add(new SelectListItem
            {
                Value = wallet.Id.ToString(),
                Text = $"{user.Name} - {wallet.WalletNumber}"
            });
        }
    }



    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detail(
        int id,
        CancellationToken cancellationToken)
    {
        var wallet = await _walletServices.GetByIdUsersAsync(
            id, cancellationToken);

        if (wallet is null)
            return NotFound();

        var user = await _userServices.GetByIdAsync(
            id, cancellationToken);

        if (user is null)
            return NotFound();

        var transactions = await _userServices.GetTransactionsByUsersIdAsync(
            id, cancellationToken);

        var model = new DetailViewModel
        {
            User = user,
            Wallet = wallet,
            Transactions = transactions
        };

        return View(model);
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

    [HttpGet("{id:int}/transfer")]
    public async Task<IActionResult> Transfer(
        int id,
        CancellationToken cancellationToken)
    {
        var wallet = await _walletServices.GetByIdUsersAsync(
            id, cancellationToken);

        if (wallet is null)
            return NotFound();

        var model = new TransferViewModel();

        await LoadWalletOptionsAsync(id, model, cancellationToken);

        return View(model);
    }

    [HttpPost("{id:int}/transfer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Transfer(
        int id,
        TransferViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadWalletOptionsAsync(id, model, cancellationToken);
            return View(model);
        }

        try
        {
            var senderWallet = await _walletServices.GetByIdUsersAsync(
                id,
                cancellationToken
            );

            if (senderWallet is null)
                return NotFound("Wallet pengirim tidak ditemukan.");

            var request = new TransferRequest(
                senderWallet.Id,
                model.ReceiverWalletId!.Value,
                model.Amount,
                model.Description
            );

            var result = await _walletServices.TransferAsync(
                request,
                cancellationToken
            );

            if (result is null)
            {
                TempData["Error"] = "Transfer gagal diproses.";
                return RedirectToAction(nameof(Detail), new { id });
            }

            TempData["Success"] = "Transfer berhasil dilakukan!";

            return RedirectToAction(nameof(Detail), new { id });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            await LoadWalletOptionsAsync(id, model, cancellationToken);

            return View(model);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            await LoadWalletOptionsAsync(id, model, cancellationToken);

            return View(model);
        }
    }
}