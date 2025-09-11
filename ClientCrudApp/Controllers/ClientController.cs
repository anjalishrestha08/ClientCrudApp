using ClientCrudApp.Models;
using ClientCrudApp.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ClientCrudApp.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientRepository _repository;

        public ClientController(IClientRepository repository)
        {
            _repository = repository;
        }

        //Index with Pagination
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            try
            {
                var clients = _repository.GetAllClients();

                // Pagination Total Count
                int totalClients = clients.Count;
                int totalPages = (int)Math.Ceiling(totalClients / (double)pageSize);

                if (totalPages == 0) totalPages = 1; // at least 1 page

                //Redirect Logic
                if (page < 1) return RedirectToAction("Index", new { page = 1 });
                if (page > totalPages) return RedirectToAction("Index", new { page = totalPages });

                //Skip and Take
                var paginatedClients = clients
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                //Pass pagination info to View 
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View(paginatedClients);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error loading clients for Index view: {e.Message}");
                TempData["Error"] = "Failed to load client data. Please try again.";
                return View(new List<Client>());
            }
        }

        //Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Client client)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repository.AddClient(client);
                    TempData["Success"] = "Client created successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error creating client: {e.Message}");
                    TempData["Error"] = "Failed to create client. Please try again.";
                }
            }
            return View(client);
        }
        //Details
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var client = _repository.GetClientById(id.Value);
            if (client == null)
            {
                TempData["Error"] = "Client not found.";
                return RedirectToAction("Index");
            }
            return View(client);
        }

        //Edit
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var client = _repository.GetClientById(id.Value);
            if (client == null)
            {
                TempData["Error"] = "Client not found.";
                return RedirectToAction("Index");
            }
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Client client)
        {
            if (id != client.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _repository.UpdateClient(client);
                    TempData["Success"] = "Client updated successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error updating client: {e.Message}");
                    TempData["Error"] = "Failed to update client. Please try again.";
                }
            }
            return View(client);
        }

        //Delete
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = _repository.GetClientById(id.Value);
            if (client == null)
            {
                TempData["Error"] = "Client not found.";
                return RedirectToAction("Index");
            }
            return View(client);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repository.DeleteClient(id);
                TempData["Success"] = "Client deleted successfully!";
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error deleting client: {e.Message}");
                TempData["Error"] = "Failed to delete client.";
            }
            return RedirectToAction("Index");
        }
    }
}
