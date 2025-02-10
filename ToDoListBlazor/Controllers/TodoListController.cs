using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;
using TodoRepository;
using TodoService;
using System.Threading.Tasks;
using System;

namespace ToDoListBlazor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoListController : ControllerBase
    {
        private readonly TodoTaskRepository mRepository;

        public TodoListController(IConfiguration configuration)
        {
            mRepository = new TodoTaskRepository(configuration.GetConnectionString("Portfolio"));
        }

        //api was added to showcase api hosting and take our leaflet marker coordinates
        //straight to the repository

        [HttpPost("save-marker")]
        public void SaveTaskMarker([FromBody] TodoTask taskMarker)
        {
            mRepository.SaveTaskMarker(taskMarker.ItemID, taskMarker.Latitude, taskMarker.Longitude);
        }
    }
}