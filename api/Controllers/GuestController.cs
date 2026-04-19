using Microsoft.AspNetCore.Mvc;
using Models;
using Repositories;

namespace Controllers
{
    [Tags("Guests"), Route("guest")]
    public class GuestController : Controller
    {
        private GuestRepository _repo;

        public GuestController(GuestRepository guestRepository)
        {
            _repo = guestRepository;
        }

        [HttpGet, Produces("application/json"), Route("")]
        public async Task<ActionResult<Guest>> GetGuests()
        {
            var guests = await _repo.GetGuests();

            return Json(guests);
        }

        [HttpPost, Produces("application/json"), Route("")]
        public async Task<ActionResult<Guest>> AddGuest([FromBody] Guest guest)
        {
            try
            {
                var registeredGuest = await _repo.CreateGuest(guest);
                return Created($"/guest/{registeredGuest.Email}", registeredGuest);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured when trying to register a new guest:");
                Console.WriteLine(ex.ToString());

                return BadRequest("Invalid guest data");
            }
        }   
    }
}
