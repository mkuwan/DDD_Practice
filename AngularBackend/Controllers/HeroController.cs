using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AngularBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroController : ControllerBase
    {

        private static Hero[] Heroes = new Hero[10]
        {
            new Hero(11, "Dr Nice"),
            new Hero(12, "Narco"),
            new Hero(13, "Bombasto"),
            new Hero(14, "Celeritas"),
            new Hero(15, "Magneta"),
            new Hero(16, "RubberMan"),
            new Hero(17, "Dynama"),
            new Hero(18, "Dr IQ"),
            new Hero(19, "Magma"),
            new Hero(20, "Tornado")
        };



        private readonly ILogger<HeroController> _logger;

        public HeroController(ILogger<HeroController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetHeroes")]
        public IEnumerable<Hero> Get()
        {
            _logger.LogInformation("HttpGet GetHeroes");

            return Heroes.ToList(); //.Take(new Range(0, 5))

        }
    }

    public class Hero
    {
        public Hero(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public int id { get; set; }
        public string name { get; set; }
    }
}
