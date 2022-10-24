using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using toDoApp.Data;
using toDoApp.Models;

namespace toDoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class ToDoController : ControllerBase
    {
        private readonly ApiDbContext _dbContext;

        //Controller Base is smaller version of Controller Class. Controller class contains MVC related along with
        //other stuff which we don't really want in web api.

        public ToDoController(ApiDbContext dbContext)
        {
            this._dbContext = dbContext;
        }


        [HttpGet]

        public async Task<IActionResult> GetItems()
        {
            var items = await _dbContext.items.ToListAsync();
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(ItemData item)
        {
            if (ModelState.IsValid) //From ASP.NET State validation
            {
                await _dbContext.AddAsync(item);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction("GetItem", new { item.Id }, item);
                //using REST API Standard.Letting user
                //know that the item has been successfully added to the DB.
                //The API Standard says once by create a item we need to return that object back with status code
                // of 201
            }
            return new JsonResult("Something Went Wrong") { StatusCode = 500 };
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id)
        {
            var item = await _dbContext.items.FindAsync(id);
            if(item != null)
            {
                return Ok(item);
            }
            return NotFound();
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateItem(int id, ItemData item)
        {
             if(id != item.Id)
            {
                return BadRequest();

            }   
             
            var exits = await _dbContext.items.FirstOrDefaultAsync(x => x.Id == id);

            if(exits != null)
            {
                exits.Title = item.Title;
                exits.Description = item.Description;
                exits.Done = item.Done;

                //WE CAN USE AUTO MAPPER 

                await _dbContext.SaveChangesAsync(); //commit changes to database
                return NoContent(); // no contest to return to the user. 
            }
            return NotFound();
        }
        [HttpDelete("{id}")]
        
        public async Task<IActionResult> DeleteItem(int id)
        {
            var exits = await _dbContext.items.FirstOrDefaultAsync(x => x.Id == id);
            if(exits != null)
            {
                _dbContext.items.Remove(exits);
                await _dbContext.SaveChangesAsync();

                return Ok(exits); 

            }
            return NotFound();
        }

    }
}
