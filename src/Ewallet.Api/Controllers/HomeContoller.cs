
using Ewallet.Api.DTOs;
using Ewallet.Api.Services;
using Ewallet.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ewallet.Api.Controllers;

[Route("")]
public sealed class HomeController : Controller
{
    private readonly IUserServices _userServices;
    private readonly IWalletServices _walletServices;

    public HomeController(IUserServices userServices, IWalletServices walletServices)
    {
        _userServices = userServices;
        _walletServices = walletServices;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var users = await _userServices.GetAllAsync(
            cancellationToken
        );

        return View(users);
    }

    [HttpGet("users/{id}")]
    public async Task<IActionResult> Detail(int id, CancellationToken cancellationToken)
    {
        var user = await _userServices.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        var wallet = await _walletServices.GetByIdUsersAsync(id,cancellationToken);

        if (wallet is null)
        {
            return NotFound("Wallet tidak ditemukan.");
        }

        var transactions = await _userServices.GetTransactionsByUsersIdAsync(id,cancellationToken);

        var model = new DetailViewModel
        {
            User = user,
            Wallet = wallet,
            Transactions = transactions
        };

        return View(model);
    }

    [HttpGet("users/{id}/edit")]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var user = await _userServices.GetByIdAsync(id, cancellationToken);

        if (user is null)
            return NotFound();

        var model = new UpdateUserRequest(
            user.Name,
            user.Email,
            user.Phone
        );

        return View(model);
    }

    [HttpPost("users/{id}/edit")]
    public async Task<IActionResult> Edit(
        int id,
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(request);

        var result = await _userServices.UpdateAsync(
            id,
            request,
            cancellationToken
        );

        if (result is null)
            return NotFound();

        TempData["Success"] = "Data pengguna berhasil diperbarui.";

        return RedirectToAction(nameof(Detail), new { id });
    }

    [HttpGet("users/create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("users/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        UserRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        try
        {
            var created = await _userServices.CreateAsync(
                request,
                cancellationToken
            );

            if (created is null)
            {
                ModelState.AddModelError(
                    nameof(request.WalletNumber),
                    "Gagal membuat user dan wallet. Nomor wallet mungkin sudah digunakan."
                );

                return View(request);
            }

            TempData["Success"] = "User dan wallet berhasil ditambahkan.";

            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message
            );

            return View(request);
        }
    }
}
