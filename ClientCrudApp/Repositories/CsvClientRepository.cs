using ClientCrudApp.Models;

namespace ClientCrudApp.Repositories
{
    public class CsvClientRepository : IClientRepository
    {
        private readonly string _filePath;
        private readonly ILogger<CsvClientRepository> _logger;

        public CsvClientRepository(ILogger<CsvClientRepository> logger)
        {
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "clients.csv");
            _logger = logger;
        }

        //Save all clients to csv
        private void SaveClientsToCsv(List<Client> clients)
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                _logger.LogInformation("Directory {dir} created for CSV storage", dir);
            }
            var lines = new List<string>
            {
               "Id,Name,Gender,CountryCode,Phone,Email,Address,Nationality,DateOfBirth,EducationBackground,PreferredModeOfContact"
            };
            lines.AddRange(clients.Select(c =>
                    $"{c.Id}," +
                    $"{c.Name.Replace(", ", " ")}," +
                    $"{c.Gender}," +
                    $"{c.CountryCode.Replace(", ", " ")}," +
                    $"{c.Phone.Replace(", ", " ")}," +
                    $"{c.Email.Replace(", ", " ")}," +
                    $"{c.Address.Replace(",", " ")}," +
                    $"{c.Nationality.Replace(",", " ")}," +
                    $"{c.DateOfBirth:yyyy-MM-dd}," +
                    $"{(c.EducationBackground ?? "").Replace(",", " ")}," +
                    $"{c.PreferredModeOfContact}"
                ));
            System.IO.File.WriteAllLines(_filePath, lines);
            _logger.LogInformation("CSV file updated with {Count} clients at {FilePath}", clients.Count, _filePath);
        }
        //Read All Clients from csv
        public List<Client> GetAllClients()
        {
            var clients = new List<Client>();

            if (!System.IO.File.Exists(_filePath))
            {
                _logger.LogWarning("CSV file not found at {FilePath}", _filePath);
                return clients;
            }

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
                    EducationBackground = cols[9],
                    PreferredModeOfContact = Enum.Parse<Client.ContactMode>(cols[10])
                });
            }
            _logger.LogInformation("Loaded {Count} clients from CSV file {FilePath}", clients.Count, _filePath);
            return clients;
        }
        //Get client by id
        public Client? GetClientById(int id)
        {
            var client = GetAllClients().FirstOrDefault(c => c.Id == id);
            if (client == null)
            {
                _logger.LogWarning("Client with ID {ClientId} not found in CSV", id);
            }
            else
            {
                _logger.LogInformation("Client with ID {ClientId} retrieved from CSV", id);
            }
            return client;
        }
        //Create Client
        public void AddClient(Client client)
        {
            var clients = GetAllClients();
            client.Id = clients.Any() ? clients.Max(c => c.Id) + 1 : 1;
            clients.Add(client);
            SaveClientsToCsv(clients);
            _logger.LogInformation("Client {ClientName} added successfully with ID {ClientId}", client.Name, client.Id);
        }

        //Edit Client
        public void UpdateClient(Client client)
        {
            var clients = GetAllClients();
            var existing = clients.FirstOrDefault(c => c.Id == client.Id);
            if (existing == null)
            {
                _logger.LogWarning("Attempt to update non-existent client with ID {ClientId}", client.Id);
                return;
            }

            existing.Name = client.Name;
            existing.Gender = client.Gender;
            existing.CountryCode = client.CountryCode;
            existing.Phone = client.Phone;
            existing.Email = client.Email;
            existing.Address = client.Address;
            existing.Nationality = client.Nationality;
            existing.DateOfBirth = client.DateOfBirth;
            existing.EducationBackground = client.EducationBackground;
            existing.PreferredModeOfContact = client.PreferredModeOfContact;

            SaveClientsToCsv(clients);
            _logger.LogInformation("Client {ClientName} with ID {ClientId} updated successfully", client.Name, client.Id);
        }
        //Delete Client
        public void DeleteClient(int id)
        {
            var clients = GetAllClients();
            var client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
            {
                clients.Remove(client);
                SaveClientsToCsv(clients);
                _logger.LogInformation("Client with ID {ClientId} deleted successfully", id);
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existent client with ID {ClientId}", id);
            }
        }
    }
}
