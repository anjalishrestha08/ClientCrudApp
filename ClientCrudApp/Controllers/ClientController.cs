using ClientCrudApp.Models;
using ClientCrudApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 

namespace ClientCrudApp.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientRepository _repository;

        private readonly ILogger<ClientController> _logger;

        public ClientController(IClientRepository repository, ILogger<ClientController> logger)
        {
            _repository = repository;
            _logger = logger;
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
                if (page < 1)
                {
                    _logger.LogWarning("Invalid page number {Page}, redirecting to 1", page);
                    return RedirectToAction("Index", new { page = 1 });
                }

                if (page > totalPages)
                {
                    _logger.LogWarning("Page exceeds total pages, redirecting to {TotalPages}", totalPages);
                    return RedirectToAction("Index", new { page = totalPages });
                }

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
                _logger.LogError(e, "Error loading clients for Index view");
                TempData["Error"] = "Failed to load client data. Please try again.";
                return View(new List<Client>());
            }
        }

        //Create
        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("Accessed Create view");
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
                    _logger.LogInformation("Client {ClientName} created successfully with ID {ClientId}", client.Name, client.Id);
                    TempData["Success"] = "Client created successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error creating client {ClientName}", client.Name);
                    TempData["Error"] = "Failed to create client. Please try again.";
                }
            }
            else
            {
                _logger.LogWarning("Invalid model state while creating client");
            }
            return View(client);
        }

        //Details
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details requested with null ID");
                return NotFound();
            }

            var client = _repository.GetClientById(id.Value);
            if (client == null)
            {
                _logger.LogWarning("Client with ID {ClientId} not found for Details", id.Value);
                TempData["Error"] = "Client not found.";
                return RedirectToAction("Index");
            }

            _logger.LogInformation("Details viewed for client {ClientName} with ID {ClientId}", client.Name, client.Id);
            return View(client);
        }

        //Edit
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit requested with null ID");
                return NotFound();
            }

            var client = _repository.GetClientById(id.Value);
            if (client == null)
            {
                _logger.LogWarning("Client with ID {ClientId} not found for Edit", id.Value);
                TempData["Error"] = "Client not found.";
                return RedirectToAction("Index");
            }

            _logger.LogInformation("Accessed Edit view for client {ClientName} with ID {ClientId}", client.Name, client.Id);
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Client client)
        {
            if (id != client.Id)
            {
                _logger.LogWarning("Edit attempted with mismatched IDs: route ID {Id} vs model ID {ClientId}", id, client.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repository.UpdateClient(client);
                    _logger.LogInformation("Client {ClientName} with ID {ClientId} updated successfully", client.Name, client.Id);
                    TempData["Success"] = "Client updated successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error updating client {ClientName} with ID {ClientId}", client.Name, client.Id);
                    TempData["Error"] = "Failed to update client. Please try again.";
                }
            }
            else
            {
                _logger.LogWarning("Invalid model state while editing client with ID {ClientId}", client.Id);
            }

            return View(client);
        }

        //Delete
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete requested with null ID");
                return NotFound();
            }

            var client = _repository.GetClientById(id.Value);
            if (client == null)
            {
                _logger.LogWarning("Client with ID {ClientId} not found for Delete", id.Value);
                TempData["Error"] = "Client not found.";
                return RedirectToAction("Index");
            }

            _logger.LogInformation("Accessed Delete confirmation view for client {ClientName} with ID {ClientId}", client.Name, client.Id);
            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repository.DeleteClient(id);
                _logger.LogInformation("Client with ID {ClientId} deleted successfully", id);
                TempData["Success"] = "Client deleted successfully!";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting client with ID {ClientId}", id);
                TempData["Error"] = "Failed to delete client.";
            }
            return RedirectToAction("Index");
        }
    }
}
