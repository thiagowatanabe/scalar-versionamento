using Microsoft.AspNetCore.Mvc;

namespace ScalarVersion.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class UserController : ControllerBase
    {
        // ============================
        // GET (comum às versões 1 e 2)
        // ============================
        [HttpGet]
        [MapToApiVersion("1.0")]
        public IActionResult Get() => Ok(new[] {
        new { Id = 1, Name = "Alice" },
        new { Id = 2, Name = "Bob" }
    });

        [HttpGet]
        [MapToApiVersion("2.0")]
        public IActionResult GetV2() => Ok(new[] {
        new { Id = 1, Name = "Alice V2" },
        new { Id = 2, Name = "Bob V2" }
    });

        // ============================
        // POST - v1 (original)
        // ============================
        [HttpPost]
        [MapToApiVersion("1.0")]
        public IActionResult CreateV1([FromBody] UserDto user)
            => CreatedAtAction(nameof(GetById), new { id = 1 }, user);

        // ============================
        // POST - v2 (nova lógica)
        // ============================
        [HttpPost]
        [MapToApiVersion("2.0")]
        public IActionResult CreateV2([FromBody] UserV2Dto user)
        {
            var fullName = $"{user.FirstName} {user.LastName}";
            return CreatedAtAction(nameof(GetById), new { id = 1 }, new { user.Id, FullName = fullName });
        }

        // ============================
        // GET by ID (comum)
        // ============================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
            => Ok(new { Id = id, Name = "User " + id });
    }

    // DTOs separados por versão
    public record UserDto(string Name); // v1

    public record UserV2Dto(int Id, string FirstName, string LastName); // v2
}
