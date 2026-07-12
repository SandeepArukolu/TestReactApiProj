using Microsoft.AspNetCore.Mvc;
using TestApiProj.MainEntity;

namespace TestApiProj.Services
{
    public interface IOperations
    {
       Task<List<User>> GetAllAsync();
        Task<string> AddUserDetails();
    }
}
