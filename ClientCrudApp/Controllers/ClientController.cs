using ClientCrudApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClientCrudApp.Controllers
{
    public class ClientController : Controller
    {
        private readonly string _filePath;

        public ClientController()
        {
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "clients.csv");
        }
        //Save single client
        private void SaveClients(Client client)
        {
            var clients = ReadClientsFromCsv();
            client.Id = clients.Any() ? clients.Max(c => c.Id) + 1 : 1;
            clients.Add(client);
            SaveClientsToCsv(clients);
        }
        //Save all clients
        private void SaveClientsToCsv(List<Client> clients)
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var lines = new List<string>
            {
               "Id,Name,Gender,CountryCode,Phone,Email,Address,Nationality,DateOfBirth,EducationBackground,PreferredModeOfContact"
            };
            lines.AddRange(clients.Select(c => $"{c.Id},{c.Name},{c.Gender},{c.CountryCode},{c.Phone},{c.Email},{c.Address},{c.Nationality},{c.DateOfBirth:yyyy-MM-dd},{c.EducationBackground},{c.PreferredModeOfContact}"));
            System.IO.File.WriteAllLines(_filePath, lines);
            TempData["Success"] = "Client added successfully";
        }
        //Read All Clients
        private List<Client> ReadClientsFromCsv()
        {
            var clients = new List<Client>();
            if (!System.IO.File.Exists(_filePath))
                return clients;
            var lines = System.IO.File.ReadAllLines(_filePath).Skip(1); // Skip header
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var cols = line.Split(',');
                clients.Add(new Client
                {
                    Id = int.Parse(cols[0]),
                    Name = cols[1],
                    Gender = Enum.Parse<Client.GenderType>(cols[2]),
                    CountryCode = cols[3],
                    Phone = cols[4],
                    Email = cols[5],
                    Address = cols[6],
                    Nationality = cols[7],
                    DateOfBirth = DateTime.Parse(cols[8]),
                    EducationBackground = cols.Length > 9 ? cols[9] : "",
                    PreferredModeOfContact = Enum.Parse<Client.ContactMode>(cols[10])
                });
            }
            return clients;
        }

        public IActionResult Index()
        {
            var clients = ReadClientsFromCsv();
            return View(clients);
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
                SaveClients(client);
                return RedirectToAction("Index");
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
            var client = ReadClientsFromCsv().FirstOrDefault(c => c.Id == id);
            if (client == null)
            {
                return NotFound();
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
            var client = ReadClientsFromCsv().FirstOrDefault(c => c.Id == id);
            if (client == null)
            {
                return NotFound();
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
                var clients = ReadClientsFromCsv();
                var existingClient = clients.FirstOrDefault(c => c.Id == id);

                if (existingClient == null) return NotFound();

                existingClient.Name = client.Name;
                existingClient.Gender = client.Gender;
                existingClient.CountryCode = client.CountryCode;
                existingClient.Phone = client.Phone;
                existingClient.Email = client.Email;
                existingClient.Address = client.Address;
                existingClient.Nationality = client.Nationality;
                existingClient.DateOfBirth = client.DateOfBirth;
                existingClient.EducationBackground = client.EducationBackground;
                existingClient.PreferredModeOfContact = client.PreferredModeOfContact;

                SaveClientsToCsv(clients);
                TempData["Success"] = "Client Updated successfully";
                return RedirectToAction("Index");
            }
            return View(client);
        }

        //Delete
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var client = ReadClientsFromCsv().FirstOrDefault(c => c.Id == id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var clients = ReadClientsFromCsv();
            var client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
            {
                clients.Remove(client);
                SaveClientsToCsv(clients);
                TempData["Success"] = "Client deleted successfully";
            }
            return RedirectToAction("Index");
        }
    }
}
