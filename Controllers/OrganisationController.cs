using HRManagement.Data;
using HRManagement.DTOs.OrganisationDTOs;
using HRManagement.Migrations;
using HRManagement.Models.Orgnisations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Controllers
{
    [Route("api/organisations")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private readonly HRDbContext _db;
        public OrganisationController(HRDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        // [Authorize]
        public async Task<ActionResult> GetOrganisations()
        {
            var organisations = await _db.Organisations.ToListAsync();


            return Ok(organisations);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrganisation([FromBody] OrganisationDTO organisationDTO)
        {
            if (organisationDTO == null)
            {
                return BadRequest(new { Message = "Invalid Organisation" });
            }
            var org = await _db.Organisations.FirstOrDefaultAsync(o => o.OrgName == organisationDTO.OrgName);
            if(org != null)
            {
                return BadRequest(new {Message = "Organisation already exists"});
            }
            var organisation = new Organisation
            {
                OrgName = organisationDTO.OrgName,
                Location = organisationDTO.Location,
                Address = organisationDTO.Address,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow

            };
            await _db.Organisations.AddAsync(organisation);
            await _db.SaveChangesAsync();
            return Ok(new { Message = "Organisation Created Successfully" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOrganisation(int id, [FromBody] OrganisationDTO organisationDTO)
        {
            var organisation = await _db.Organisations.FindAsync(id);
            if (organisation == null)
            {
                return NotFound(new { Message = "Organiation not found" });
            }
            organisation.OrgName = organisationDTO.OrgName;
            organisation.Address = organisationDTO.Address;
            organisation.Location = organisationDTO.Location;
            organisation.ModifiedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new { Message = "Organisation updated successfully" });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetOrganisation(int id)
        {
            var organisation = await _db.Organisations.FindAsync(id);
            if (organisation == null)
            {
                return NotFound(new { Message = "Organisation not found" });
            }
            var org = new OrganisationResponseDTO
            {
                Id = organisation.Id,
                OrgName = organisation.OrgName,
                Location = organisation.Location,
                Address = organisation.Address
            };
            return Ok(new { data = org });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrganisation(int id)
        {
            var organisation = await _db.Organisations.FindAsync(id);
            if (organisation == null)
            {
                return NotFound(new { Message = "Organisation not found" });
            }
            _db.Organisations.Remove(organisation);
            await _db.SaveChangesAsync();
            return Ok(new { Message = "Organisation Deleted Succussfully" });
        }
    }
}
