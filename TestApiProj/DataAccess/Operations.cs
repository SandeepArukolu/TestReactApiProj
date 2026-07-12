using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using TestApiProj.MainEntity;
using TestApiProj.Services;

namespace TestApiProj.DataAccess
{
    public class Operations: IOperations
    {
        private readonly HttpClient _httpClient;

        private readonly MyDbContext _context;

        public Operations(HttpClient httpClient, MyDbContext dbContext)
        {
          _httpClient = httpClient;
            _context = dbContext;
        }
        public async Task<List<User>> GetAllAsync()
        {
            string url = "https://jsonplaceholder.typicode.com/users";
            HttpResponseMessage response = await _httpClient.GetAsync(url);         
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<User>>(responseBody).ToList();
            }
            return new List<User>();
        }


      
        public async Task<string>  AddUserDetails()
        {
            var list = await GetAllAsync();
            var result = list.Select(user => new User
            {
                email = user.email,
                Password = "sa1234",
                UserRole= "Admin"
            }).ToList();

            foreach (var item in result)
            {
                await _context.Users.AddAsync(item);
            }

            await _context.SaveChangesAsync();
            return "Added Successfully";
        }
    }
}

