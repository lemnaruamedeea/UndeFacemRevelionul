﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UndeFacemRevelionul.ContextModels;
using UndeFacemRevelionul.Models;
using UndeFacemRevelionul.ViewModels;
using System.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Threading;


namespace UndeFacemRevelionul.Controllers;

public class PartierController : Controller
{
    private readonly RevelionContext _context;
    private readonly ILogger<AccountController> _logger;

    public PartierController(RevelionContext context, ILogger<AccountController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Metoda AddPoints ar trebui să fie definită ca un POST request:
    [HttpPost]
    public IActionResult AddPoints(int points)
    {
        var currentUserId = GetCurrentUserId();

        // Găsim partier-ul asociat utilizatorului logat
        var partier = _context.Partiers.FirstOrDefault(p => p.UserId == currentUserId);

        if (partier == null)
        {
            TempData["ErrorMessage"] = "Nu există un partier asociat utilizatorului logat.";
            return RedirectToAction("Index", "Home");
        }

        // Actualizăm punctele
        partier.Points += points;

        // Salvăm modificările în baza de date
        _context.SaveChanges();

        TempData["SuccessMessage"] = $"Ai câștigat {points} puncte!";
        return RedirectToAction("Dashboard");
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        var currentUserId = GetCurrentUserId();

        // Găsim partier-ul asociat utilizatorului logat
        var partier = _context.Partiers.FirstOrDefault(p => p.UserId == currentUserId);

        if (partier == null)
        {
            TempData["ErrorMessage"] = "Nu există un partier asociat utilizatorului logat.";
            return RedirectToAction("Index", "Home");
        }

        // Găsim toate petrecerile asociate utilizatorului
        var parties = _context.PartyPartiers
            .Where(pp => pp.PartierId == partier.Id)
            .Select(pp => pp.Party)
            .ToList();

        // Găsim rangul utilizatorului logat pe baza punctelor
        var rank = _context.Ranks
            .FirstOrDefault(r => partier.Points >= r.MinPoints && partier.Points <= r.MaxPoints);

        // Transmitem datele către View prin ViewBag
        ViewBag.Parties = parties;
        ViewBag.RankName = rank?.Name ?? "N/A";
        ViewBag.PartierPoints = partier.Points;

        return View();
    }

    [Authorize]
    public IActionResult WheelOfFortune()
    {
        return View();
    }


    //// GET: Wheel of Fortune
    //[HttpGet]
    //public IActionResult WheelOfFortune()
    //{
    //    var currentUserId = GetCurrentUserId();

    //    // Găsim partier-ul asociat utilizatorului logat
    //    var partier = _context.Partiers.FirstOrDefault(p => p.UserId == currentUserId);

    //    if (partier == null)
    //    {
    //        TempData["ErrorMessage"] = "Nu există un partier asociat utilizatorului logat.";
    //        return RedirectToAction("Index", "Home");
    //    }

    //    // Elimini verificarea pentru 24 de ore
    //    // Acum nu mai există restricții de rotație zilnică
    //    // Logica pentru a face un spin (poți să adaugi puncte aici)
    //    Random random = new Random();
    //    int spunPoints = random.Next(10, 101); // Exemplu: câștigă între 10 și 100 puncte
    //    partier.Points += spunPoints;

    //    partier.LastSpinDate = DateTime.Now; // Actualizează data ultimului spin

    //    _context.SaveChanges(); // Salvează modificările în baza de date

    //    TempData["SuccessMessage"] = $"Ai câștigat {spunPoints} puncte! Total puncte: {partier.Points}";

    //    return RedirectToAction("Dashboard");
    //}

    //[HttpPost]
    //public IActionResult SpinWheel()
    //{
    //    var currentUserId = GetCurrentUserId();  // Obține ID-ul utilizatorului logat

    //    // Găsim partier-ul asociat utilizatorului logat
    //    var partier = _context.Partiers.FirstOrDefault(p => p.UserId == currentUserId);

    //    if (partier == null)
    //    {
    //        TempData["ErrorMessage"] = "Nu există un partier asociat utilizatorului logat.";
    //        return RedirectToAction("Index", "Home");
    //    }

    //    // Elimini verificarea pentru data ultimei rotiri
    //    // Nu mai există restricții, astfel încât nu mai verificăm dacă utilizatorul a rotit deja roata azi

    //    // Actualizăm data ultimei rotiri
    //    partier.LastSpinDate = DateTime.Now;

    //    // Logica pentru calculul premiului și punctelor câștigate
    //    var reward = GetWheelReward(); // O funcție care întoarce punctele câștigate
    //    partier.Points += reward;

    //    // Salvează modificările în baza de date
    //    _context.SaveChanges();

    //    // Mesaj de succes
    //    TempData["SuccessMessage"] = $"Ai câștigat {reward} puncte!";

    //    // Redirecționăm utilizatorul înapoi la dashboard
    //    return RedirectToAction("Dashboard");
    //}

    //private int GetWheelReward()
    //{
    //    // Logica pentru determinarea premiului roții
    //    // De exemplu, generăm un număr aleator pentru puncte
    //    var random = new Random();
    //    return random.Next(10, 101); // Puncte între 10 și 100
    //}



    // GET: CreateParty
    [HttpGet]
    public IActionResult CreateParty()
    {
        return View(new CreatePartyViewModel());
    }

    // POST: CreateParty
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateParty(CreatePartyViewModel model)
    {
        if (model.TotalBudget <= 0)
        {
            TempData["ErrorMessage"] = "Bugetul total trebuie să fie mai mare decât 0!";
            return View(model);
        }

        if (ModelState.IsValid)
        {
            var party = new PartyModel
            {
                Name = model.Name,
                TotalBudget = model.TotalBudget,
                RemainingBudget = model.TotalBudget, // Inițial, bugetul rămas este egal cu cel total
                Date = model.Date,
                TotalPoints = 0,
                LocationId = model.LocationId,
                FoodMenuId = model.FoodMenuId,
                PartyUsers = new List<PartyPartierModel>()
            };

            _context.Parties.Add(party);
            _context.SaveChanges();

            var currentUserId = GetCurrentUserId();
            var partier = _context.Partiers.FirstOrDefault(p => p.UserId == currentUserId);

            if (partier == null)
            {
                _logger.LogError($"Partier cu UserId {currentUserId} nu a fost găsit.");
                return RedirectToAction("Error", "Home");
            }

            var partyUser = new PartyPartierModel
            {
                PartyId = party.Id,
                PartierId = partier.Id
            };

            var defaultTasks = new List<TaskModel>
            {
                new TaskModel { PartyId = party.Id, Name = "Rezervare locație", IsCompleted = false, Points = 100 },
                new TaskModel { PartyId = party.Id, Name = "Alegere meniu", IsCompleted = false, Points = 100 },
                new TaskModel { PartyId = party.Id, Name = "Playlist", IsCompleted = false, Points = 100 }
            };

            _context.Tasks.AddRange(defaultTasks);

            party.PartyUsers.Add(partyUser);
            _context.PartyPartiers.Add(partyUser);
            _context.SaveChanges();

            UpdatePartyTotalPoints(party.Id);

            return RedirectToAction("Dashboard", "Partier");
        }

        return View(model);
    }


    private int GetCurrentUserId()
    {
        // Verifică dacă utilizatorul este autentificat
        if (!User.Identity.IsAuthenticated)
        {
            _logger.LogError("User is not authenticated.");
            throw new InvalidOperationException("User is not authenticated.");
        }

        // Adaugă log pentru a verifica ce claim-uri sunt disponibile
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        foreach (var claim in claims)
        {
            _logger.LogInformation($"Claim Type: {claim.Type}, Value: {claim.Value}");
        }

        // Căutăm Claim-ul pentru NameIdentifier
        var userIdClaim = User.FindFirst("UserId");


        if (userIdClaim == null)
        {
            _logger.LogError("User does not have a valid ID claim.");
            throw new InvalidOperationException("User does not have a valid ID claim.");
        }

        // Returnează ID-ul utilizatorului
        return int.Parse(userIdClaim.Value);
    }

    public IActionResult PartyDetails(int id)
    {
        var currentUserId = GetCurrentUserId();

        var party = _context.Parties
            .Include(p => p.Tasks)
            .Include(p => p.FoodMenu) // Încarcă meniul asociat
            .Include(p => p.Location) // Încarcă locația asociată
            .Include(p => p.PartyUsers)  // Include utilizatorii petrecerii
            .ThenInclude(pu => pu.Partier)  // Include informațiile despre partieri
            .ThenInclude(pa => pa.User)   // Încarcă detaliile utilizatorului
            .FirstOrDefault(p => p.Id == id);

        if (party == null)
        {
            return NotFound();
        }

        var currentPartier = _context.Partiers.FirstOrDefault(p => p.UserId == currentUserId);

        if (currentPartier == null)
        {
            TempData["ErrorMessage"] = "Partierul nu a fost găsit.";
            return RedirectToAction("Dashboard", "Partier");
        }

        // Include playlist cu melodii
        var playlist = _context.Playlists
            .Include(pl => pl.PlaylistSongs)
                .ThenInclude(ps => ps.Song) // Include detaliile melodiei
            .FirstOrDefault(pl => pl.PartyId == id);

        if (playlist != null && playlist.PlaylistSongs != null)
        {
            Debug.WriteLine($"Number of songs in playlist: {playlist.PlaylistSongs.Count}");

            var playlistSongUrls = playlist.PlaylistSongs
                .Where(ps => !string.IsNullOrEmpty(ps.Song.SongPath)) // Verificăm ca SongPath să nu fie null
                .Select(ps => ps.Song.SongPath)
                .ToList();

            ViewBag.SerializedPlaylistSongs = JsonConvert.SerializeObject(playlistSongUrls);
        }
        else
        {
            ViewBag.SerializedPlaylistSongs = "[]"; // Dacă nu sunt melodii, trimitem o listă goală
        }


        ViewBag.Playlist = playlist;


        var assignedTasks = party.Tasks.Where(t => t.PartierId == currentPartier.Id).ToList();

        var tasks = party.Tasks.ToList();
        ViewBag.AssignedTasks = assignedTasks;
        ViewBag.CurrentPartier = currentPartier;
        ViewBag.AllTasks = _context.Tasks.Where(x => x.PartyId == id).ToList();

        var partyPartiers = _context.PartyPartiers
            .Where(pp => pp.PartyId == id)
            .Include(pp => pp.Partier)
            .ThenInclude(p => p.User)
            .Select(pp => pp.Partier)
            .ToList();

        var partierNames = partyPartiers.Select(partier => partier.User.Name).ToList();
        ViewBag.PartierNames = partierNames;

        var superstitions = _context.Superstitions
        .Where(s => s.PartyId == id)
        .ToList();
        ViewBag.Superstitions = superstitions;

        UpdatePartyTotalPoints(id);

        return View(party);
    }

    public IActionResult AddSuperstition(int partyId)
    {
        var model = new AddSuperstitionViewModel
        {
            PartyId = partyId
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult AddSuperstition(AddSuperstitionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid input data.";
            return View(model);
        }

        var currentPartier = _context.Partiers.FirstOrDefault(p => p.UserId == GetCurrentUserId());
        if (currentPartier == null)
        {
            TempData["ErrorMessage"] = "The current partier could not be found.";
            return RedirectToAction("PartyDetails", new { id = model.PartyId });
        }

        var party = _context.Parties.FirstOrDefault(p => p.Id == model.PartyId);
        if (party == null)
        {
            TempData["ErrorMessage"] = "Party not found.";
            return RedirectToAction("Dashboard", "Partier");
        }

        var superstition = new SuperstitionModel
        {
            Name = model.Name,
            Points = model.Points,
            PartyId = model.PartyId,
            PartierId = currentPartier.Id,
            IsCompleted = false
        };

        try
        {
            _context.Superstitions.Add(superstition);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Superstition added successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error while adding superstition: " + ex.Message;
        }

        return RedirectToAction("PartyDetails", new { id = model.PartyId });
    }




    [HttpPost]
    public async Task<IActionResult> CompleteSuperstition(SuperstitionCompletionViewModel model)
    {
        var superstition = await _context.Superstitions
            .FirstOrDefaultAsync(s => s.Id == model.SuperstitionId && s.PartyId == model.PartyId);

        if (superstition == null)
        {
            // Superstiția nu există
            return NotFound();
        }

        // Dacă superstiția este completată
        if (!superstition.IsCompleted)
        {
            // Dacă există o imagine
            if (model.Image != null && model.Image.Length > 0)
            {
                // Salvează imaginea pe server
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", model.Image.FileName);

                // Salvează imaginea în server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                // Salvează calea imaginii
                superstition.ImagePath = "/images/" + model.Image.FileName;
            }

            // Marcam superstitiile ca completate si adaugam punctele
            superstition.IsCompleted = true;

            // Adăugăm punctele superstitiilor la totalul petrecerii
            var party = await _context.Parties
                .FirstOrDefaultAsync(p => p.Id == model.PartyId);

            if (party != null)
            {
                party.TotalPoints += superstition.Points;
                await _context.SaveChangesAsync();
            }
        }

        return RedirectToAction("PartyDetails", new { id = model.PartyId });
    }




    // GET: EditParty
    [HttpGet]
    public IActionResult EditParty(int id)
    {
        var party = _context.Parties.FirstOrDefault(p => p.Id == id);

        if (party == null)
        {
            return NotFound();
        }

        return View(party);
    }

    // POST: EditParty
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditParty(PartyModel updatedParty)
    {
        var party = _context.Parties
            .Include(p => p.Location)
            .Include(p => p.FoodMenu)
            .FirstOrDefault(p => p.Id == updatedParty.Id);

        if (party == null)
        {
            return NotFound();
        }

        if (updatedParty.TotalBudget <= 0)
        {
            TempData["ErrorMessage"] = "Bugetul total trebuie să fie mai mare decât 0!";
            return View(updatedParty);
        }

        party.Name = updatedParty.Name;
        party.TotalBudget = updatedParty.TotalBudget;
        party.Date = updatedParty.Date;

        float menuPrice = party.FoodMenu?.Price ?? 0;
        float locationPrice = party.Location?.Price ?? 0;
        float newRemainingBudget = updatedParty.TotalBudget - (menuPrice + locationPrice);

        if (newRemainingBudget < 0)
        {
            TempData["ErrorMessage"] = "Bugetul rămas nu poate fi negativ! Ajustează bugetul total sau elimină locația/meniul.";
            return View(updatedParty);
        }

        party.RemainingBudget = newRemainingBudget;

        _context.SaveChanges();

        return RedirectToAction("PartyDetails", new { id = party.Id });
    }


    // GET: DeleteParty (Confirmare)
    [HttpGet]
    public IActionResult DeleteParty(int id)
    {
        var party = _context.Parties.FirstOrDefault(p => p.Id == id);

        if (party == null)
        {
            return NotFound();
        }

        return View(party);
    }

    // POST: DeleteParty
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePartyConfirmed(int id)
    {
        // Găsește petrecerea
        var party = _context.Parties.FirstOrDefault(p => p.Id == id);
        if (party == null)
        {
            return NotFound();
        }

        // Șterge înregistrările asociate din PartyPartiers
        var partyPartiers = _context.PartyPartiers.Where(pp => pp.PartyId == id).ToList();
        _context.PartyPartiers.RemoveRange(partyPartiers);

        var partyTasks = _context.Tasks.Where(t => t.PartyId == id).ToList();
        _context.Tasks.RemoveRange(partyTasks);

        // Șterge petrecerea
        _context.Parties.Remove(party);

        // Salvează modificările
        _context.SaveChanges();

        return RedirectToAction("Dashboard");
    }
    [HttpGet]
    public IActionResult AddMember(int partyId)
    {
        var party = _context.Parties.FirstOrDefault(p => p.Id == partyId);
        if (party == null)
        {
            return NotFound("Petrecerea nu a fost găsită.");
        }

        ViewBag.PartyId = partyId;
        ViewBag.PartyName = party.Name;
        return View();
    }

    [HttpGet]
    public IActionResult ListMenus(int partyId, string search = "", float? minPrice = null, float? maxPrice = null, float? rating = null)
    {
        var menusQuery = _context.FoodMenus
            .Include(m => m.Provider)
            .ThenInclude(p => p.User)
            .AsQueryable(); // Pregătim interogarea

        // Aplicăm filtrele doar dacă utilizatorul a completat câmpurile
        if (!string.IsNullOrWhiteSpace(search))
        {
            menusQuery = menusQuery.Where(m => m.Name.Contains(search) || m.Description.Contains(search));
        }

        if (minPrice.HasValue)
        {
            menusQuery = menusQuery.Where(m => m.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            menusQuery = menusQuery.Where(m => m.Price <= maxPrice.Value);
        }

        if (rating.HasValue)
        {
            menusQuery = menusQuery.Where(m => m.Rating >= rating.Value);
        }

        var menus = menusQuery.ToList(); // Execută interogarea

        var party = _context.Parties
            .Include(p => p.PartyUsers)
            .ThenInclude(pu => pu.Partier) // Include participanții
            .Include(p => p.FoodMenu) // Include meniul pentru preț
            .FirstOrDefault(p => p.Id == partyId);

        if (party == null)
        {
            return NotFound("Party not found.");
        }

        

        // Calculăm totalul punctelor
        int totalPoints = party.PartyUsers?.Sum(pu => pu.Partier.Points) ?? 0;

        // Aplicăm reducerea dacă punctele depășesc 10.000
        float? discountedPrice = null;
        if (totalPoints > 10000 && totalPoints < 15000 && party.FoodMenu != null)
        {
            discountedPrice = party.FoodMenu.Price * 0.9f; // Reducere de 10%
        }

        if (totalPoints > 15000 && party.FoodMenu != null)
        {
            discountedPrice = party.FoodMenu.Price * 0.85f; // Reducere de 15%
        }

        var viewModel = new ListMenusViewModel
        {
            PartyId = partyId,
            Menus = menus,
            TotalPoints = totalPoints,
            DiscountedPrice = discountedPrice
        };

        return View(viewModel);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddMenuToParty(int partyId, int menuId)
    {
        var party = _context.Parties
            .Include(p => p.Location)
            .Include(p => p.PartyUsers)
                .ThenInclude(pu => pu.Partier)
            .Include(p => p.FoodMenu)
            .FirstOrDefault(p => p.Id == partyId);

        if (party == null)
            return NotFound();

        if (party != null)
        {
            party.FoodMenuId = menuId;

            // Găsim task-ul "Alegere meniu"
            var task = _context.Tasks.FirstOrDefault(t => t.PartyId == partyId && t.Name == "Alegere meniu");
            var partier = _context.Partiers.FirstOrDefault(p => p.Id == task.PartierId);
            if (task != null)
            {
                task.IsCompleted = true; // Bifăm task-ul
                if (partier != null)
                {
                    partier.Points += task.Points;
                }
                else {
                    TempData["ErrorMessage"] = "Trebuie sa iti iei task!";
                    return RedirectToAction("ListMenus", new { partyId });
                }
            }

        }

        var menu = _context.FoodMenus.Find(menuId);
        if (menu == null)
            return NotFound();

        // Calculăm totalul punctelor
        int totalPoints = party.PartyUsers?.Sum(pu => pu.Partier.Points) ?? 0;

        // Aplicăm reducerea dacă punctele depășesc 10.000
        float? discountedPrice = null;
        if (totalPoints > 10000 && totalPoints < 15000 && party.FoodMenu != null)
        {
            discountedPrice = party.FoodMenu.Price * 0.9f; // Reducere de 10%
        }

        if (totalPoints > 15000 && party.FoodMenu != null)
        {
            discountedPrice = party.FoodMenu.Price * 0.85f; // Reducere de 15%
        }

        // Asigurăm că se folosește prețul redus dacă există
        float menuPrice = discountedPrice ?? menu.Price;
        float locationPrice = party.Location?.Price ?? 0;
        float newRemainingBudget = party.TotalBudget - menuPrice - locationPrice;

        
        if (newRemainingBudget < 0)
        {
            TempData["ErrorMessage"] = "Nu ai suficient buget pentru acest meniu!";
            return RedirectToAction("ListMenus", new { partyId });
        }

        party.FoodMenuId = menu.Id;
        party.RemainingBudget = newRemainingBudget;

        _context.SaveChanges();

        return RedirectToAction("PartyDetails", new { id = partyId });
    }



    [HttpGet]
    public IActionResult ListLocations(int partyId, string search = "", float? minPrice = null, float? maxPrice = null, int? capacity = null, float? rating = null)
    {
        var locationsQuery = _context.Locations
            .Include(l => l.Provider)
            .ThenInclude(p => p.User)
            .AsQueryable(); // Transformă interogarea într-una flexibilă

        // Aplicăm filtrele doar dacă sunt setate
        if (!string.IsNullOrWhiteSpace(search))
        {
            locationsQuery = locationsQuery.Where(l => l.Name.Contains(search) || l.Address.Contains(search));
        }

        if (minPrice.HasValue)
        {
            locationsQuery = locationsQuery.Where(l => l.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            locationsQuery = locationsQuery.Where(l => l.Price <= maxPrice.Value);
        }

        if (capacity.HasValue)
        {
            locationsQuery = locationsQuery.Where(l => l.Capacity >= capacity.Value);
        }

        if (rating.HasValue)
        {
            locationsQuery = locationsQuery.Where(l => l.Rating >= rating.Value);
        }

        var locations = locationsQuery.ToList(); // Execută interogarea în baza de date

        var party = _context.Parties
            .Include(p => p.PartyUsers)
            .ThenInclude(pu => pu.Partier)
            .Include(p => p.Location)
            .FirstOrDefault(p => p.Id == partyId);

        if (party == null)
        {
            return NotFound("Party not found.");
        }

        // Calculează totalul punctelor
        int totalPoints = party.PartyUsers?.Sum(pu => pu.Partier.Points) ?? 0;

        // Aplică reducerea dacă punctele depășesc 10.000
        float? discountedPrice = null;
        if (totalPoints > 10000 && totalPoints < 15000 && party.Location != null)
        {
            discountedPrice = party.Location.Price * 0.9f;
        }

        if (totalPoints > 15000 && party.Location != null)
        {
            discountedPrice = party.Location.Price * 0.85f;
        }

        var viewModel = new ListLocationsViewModel
        {
            PartyId = partyId,
            Locations = locations,
            TotalPoints = totalPoints,
            DiscountedPrice = discountedPrice
        };

        return View(viewModel);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddLocationToParty(int partyId, int locationId)
    {
        var party = _context.Parties
            .Include(p => p.FoodMenu)
            .Include(p => p.PartyUsers)
                .ThenInclude(pu => pu.Partier)
            .Include(p => p.Location)
            .FirstOrDefault(p => p.Id == partyId);

        if (party == null)
            return NotFound();

        if (party != null)
        {
            party.LocationId = locationId;

            // Găsim task-ul "Rezervare locație"
            var task = _context.Tasks.FirstOrDefault(t => t.PartyId == partyId && t.Name == "Rezervare locație");
            var partier = _context.Partiers.FirstOrDefault(p => p.Id == task.PartierId);
            if (task != null)
            {
                task.IsCompleted = true; // Bifăm task-ul
                if (partier != null)
                {
                    partier.Points += task.Points;
                }
                else
                {
                    TempData["ErrorMessage"] = "Trebuie sa iti iei task!";
                    return RedirectToAction("ListLocations", new { partyId });
                }
            }

        }

        var location = _context.Locations.Find(locationId);
        if (location == null)
            return NotFound();

        // Calculează totalul punctelor
        int totalPoints = party.PartyUsers?.Sum(pu => pu.Partier.Points) ?? 0;

        // Aplică reducerea dacă punctele depășesc 10.000
        float? discountedPrice = null;
        if (totalPoints > 10000 && totalPoints < 15000 && party.Location != null)
        {
            discountedPrice = party.Location.Price * 0.9f;
        }

        if (totalPoints > 15000 && party.Location != null)
        {
            discountedPrice = party.Location.Price * 0.85f;
        }

        float locationPrice = discountedPrice ?? location.Price;
        float menuPrice = party.FoodMenu?.Price ?? 0;
        float newRemainingBudget = party.TotalBudget - menuPrice - locationPrice;
        
        if (newRemainingBudget < 0)
        {
            TempData["ErrorMessage"] = "Nu ai suficient buget pentru această locație!";
            return RedirectToAction("ListLocations", new { partyId });
        }

        party.LocationId = location.Id;
        party.RemainingBudget = newRemainingBudget;

        _context.SaveChanges();

        return RedirectToAction("PartyDetails", new { id = partyId });
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddMember(int partyId, string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            TempData["ErrorMessage"] = "Adresa de email nu poate fi goală.";
            return RedirectToAction("AddMember", new { partyId });
        }

        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Nu există niciun utilizator cu această adresă de email.";
            return RedirectToAction("AddMember", new { partyId });
        }

        var partier = _context.Partiers.FirstOrDefault(p => p.UserId == user.Id);
        if (partier == null)
        {
            TempData["ErrorMessage"] = "Utilizatorul găsit nu este un partier.";
            return RedirectToAction("AddMember", new { partyId });
        }

        var alreadyMember = _context.PartyPartiers.Any(pp => pp.PartyId == partyId && pp.PartierId == partier.Id);
        if (alreadyMember)
        {
            TempData["ErrorMessage"] = "Partierul este deja membru al acestei petreceri.";
            return RedirectToAction("AddMember", new { partyId });
        }

        var partyPartier = new PartyPartierModel
        {
            PartyId = partyId,
            PartierId = partier.Id
        };

        _context.PartyPartiers.Add(partyPartier);
        _context.SaveChanges();

        UpdatePartyTotalPoints(partyId);

        TempData["SuccessMessage"] = "Partierul a fost adăugat cu succes și totalul punctelor a fost actualizat.";

        return RedirectToAction("PartyDetails", new { id = partyId });
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveMember(int partyId, int partierId)
    {
        var partyPartier = _context.PartyPartiers
            .FirstOrDefault(pp => pp.PartyId == partyId && pp.PartierId == partierId);

        if (partyPartier == null)
        {
            return NotFound();
        }

        _context.PartyPartiers.Remove(partyPartier);
        _context.SaveChanges();

        UpdatePartyTotalPoints(partyId);
        TempData["SuccessMessage"] = "Membrul a fost șters și totalul punctelor a fost actualizat.";


        return RedirectToAction("PartyDetails", new { id = partyId });
    }



    private void UpdatePartyTotalPoints(int partyId)
    {
        // Găsim petrecerea și includem superstitiile
        var party = _context.Parties
            .Include(p => p.PartyUsers) // Relația PartyPartiers
                .ThenInclude(pp => pp.Partier)
            .Include(p => p.Superstitions) // Include superstitiile asociate petrecerii
            .FirstOrDefault(p => p.Id == partyId);

        if (party == null)
        {
            throw new Exception("Petrecerea nu a fost găsită.");
        }

        // Calculăm punctele totale ale partierilor
        int totalPoints = party.PartyUsers.Sum(pu => pu.Partier.Points);

        // Adăugăm punctele superstitiilor completate
        var completedSuperstitionsPoints = party.Superstitions
            .Where(s => s.IsCompleted)
            .Sum(s => s.Points);

        // Debug: Adăugăm loguri pentru a vedea ce se întâmplă
        Debug.WriteLine($"Total Points from Party Users: {totalPoints}");
        Debug.WriteLine($"Completed Superstitions Points: {completedSuperstitionsPoints}");

        // Adăugăm punctele superstitiilor completate
        totalPoints += completedSuperstitionsPoints;

        // Debug: Verificăm totalul final de puncte
        Debug.WriteLine($"Final Total Points for Party {partyId}: {totalPoints}");

        // Setăm punctele totale ale petrecerii
        party.TotalPoints = totalPoints;

        // Salvăm schimbările în baza de date
        _context.SaveChanges();
    }


    [HttpGet]
    public IActionResult PartyTasks(int id)
    {
        // Preluăm petrecerea din baza de date, incluzând task-urile asociate
        var tasks = _context.Tasks.Where(x => x.PartyId == id).ToList();

        if (tasks == null)
        {
            return NotFound(); // Dacă nu găsim petrecerea, returnăm un 404
        }

        // Trimitem task-urile către view
        return View(tasks);
    }


    [HttpPost]
    public IActionResult ClaimTask(int taskId, int partyId)
    {
        var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId && t.PartyId == partyId);

        if (task != null)
        {
            // Preluăm utilizatorul logat
            var currentUserId = GetCurrentUserId(); // ID-ul userului logat
            var currentPartier = _context.Partiers
                                         .FirstOrDefault(p => p.UserId == currentUserId);

            if (currentPartier != null)
            {
                // Asociem task-ul userului logat
                task.PartierId = currentPartier.Id;
                _context.SaveChanges();
            }
        }

        return RedirectToAction("PartyDetails", new { id = partyId });
    }

    [HttpGet]
    public IActionResult AddTask(int partyId)
    {
        var viewModel = new AddTaskViewModel
        {
            PartyId = partyId
        };
        return View(viewModel);
    }

    [HttpPost]
    public IActionResult AddTask(AddTaskViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var task = new TaskModel
            {
                PartyId = viewModel.PartyId,
                Name = viewModel.Name,
                Points = viewModel.Points,
                IsCompleted = false // Setăm implicit la "necompletat"
            };

            _context.Tasks.Add(task);
            _context.SaveChanges();

            return RedirectToAction("PartyDetails", new { id = viewModel.PartyId });
        }

        return View(viewModel); // Returnează același view pentru erori
    }



    [HttpPost]
    public IActionResult ToggleTaskCompletion(int taskId, int partyId)
    {
        var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId && t.PartyId == partyId);

        if (task != null && task.PartierId.HasValue)
        {
            // Preluăm Partier-ul asociat task-ului
            var partier = _context.Partiers.FirstOrDefault(p => p.Id == task.PartierId);

            if (partier != null)
            {

                if (task.Name != "Alegere meniu" && task.Name != "Rezervare locație" && task.Name != "Playlist")
                {
                    // Verificăm și schimbăm starea task-ului
                    if (!task.IsCompleted)
                    {
                        // Dacă task-ul nu este completat, îl completăm și adăugăm punctele
                        task.IsCompleted = true;
                        partier.Points += task.Points; // Adaugăm punctele la Partier
                    }
                    else
                    {
                        // Dacă era deja completat, îl setăm ca necompletat și scădem punctele
                        task.IsCompleted = false;
                        partier.Points -= task.Points; // Scădem punctele din Partier
                    }
                }

                else
                {
                    TempData["ErrorMessage"] = "Nu poți schimba starea acestui task!";
                }

                _context.SaveChanges();
            }
        }
        UpdatePartyTotalPoints(partyId);
        return RedirectToAction("PartyDetails", new { id = partyId });
    }





}
