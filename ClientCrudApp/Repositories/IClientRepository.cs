using ClientCrudApp.Models;

namespace ClientCrudApp.Repositories
{
    public interface IClientRepository
    {
        List<Client> GetAllClients();
        Client? GetClientById(int id);
        void AddClient(Client client);
        void UpdateClient(Client client);
        void DeleteClient(int id);
    }
}
