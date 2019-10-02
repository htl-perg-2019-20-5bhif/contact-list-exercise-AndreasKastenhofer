using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace contact_list_api.Controllers
{ 
    public class Person
    {
        [Required]
        public int id { get; set; }

        [MaxLength(50)]
        public string firstName { get; set; }

        [MaxLength(50)]
        public string lastName { get; set; }

        [Required]
        [MaxLength(250)]
        public string email { get; set; }
}
    [ApiController]
    [Route("api/contact-list")]
    public class ContactListController : ControllerBase
    {
        private static readonly List<Person> contacts = 
            new List<Person>
            {
            new Person {id = 0, firstName="Max", lastName="Mustermann", email="max.mustermann@gmail.com"}
            };

        private readonly ILogger<ContactListController> _logger;

        public ContactListController(ILogger<ContactListController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAllContacts()
        {
            return Ok(contacts);
        }

        [HttpGet]
        [Route("findByName")]
        public IActionResult GetPersonFromName([FromQuery] string nameFilter)
        {
            if(nameFilter == null)
            {
                return BadRequest("Invalid Name");
            }
            IEnumerable<Person> results =
                from person in contacts
                where person.firstName.Equals(nameFilter) ||
                person.lastName.Equals(nameFilter)
                select person;

            if(results.Count() > 0)
            {
                return Ok(results);
            }
            return NotFound("Person not found!");
        }

        [HttpPost]
        public IActionResult AddItem([FromHeader] Person newPerson)
        {
            if(newPerson == null || newPerson.id < 0 || newPerson.email == null)
            {
                return BadRequest("Invalid arguments!");
            }

            var value = contacts.Find(person => person.id == newPerson.id);
            if(value != null)
            {
                return BadRequest("Person already exists!");
            }

            contacts.Add(newPerson);
            return Created("Created Person", newPerson);
        }

        [HttpDelete]
        [Route("{index}")]
        public IActionResult DeleteItem(int index)
        {
            if (index < 0)
            {
                BadRequest("Invalid ID");
            }
            var value = contacts.Find(person => person.id == index);
            if (value != null)
            {
                contacts.RemoveAt(index);
                return NoContent();
            }
            return NotFound("Person not found!");
        }

    }
}
