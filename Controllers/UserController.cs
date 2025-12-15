using ApungLourdesWebApi.DTOs;
using ApungLourdesWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApungLourdesWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service) { _service = service; }

        [HttpGet] public async Task<ActionResult<IEnumerable<UserDto>>> GetAll() => Ok(await _service.GetAllUsersAsync());
        [HttpGet("{id}")] public async Task<ActionResult<UserDto>> Get(int id) => (await _service.GetUserByIdAsync(id)) is { } user ? Ok(user) : NotFound();
        [HttpPost] public async Task<ActionResult<UserDto>> Create(UserDto dto) => Ok(await _service.CreateUserAsync(dto));
        [HttpPut("{id}")] public async Task<ActionResult<UserDto>> Update(int id, UserDto dto) => (await _service.UpdateUserAsync(id, dto)) is { } updated ? Ok(updated) : NotFound();
        [HttpDelete("{id}")] public async Task<IActionResult> Delete(int id) => await _service.DeleteUserAsync(id) ? NoContent() : NotFound();
    }
}
