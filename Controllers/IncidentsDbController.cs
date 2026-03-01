using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IncidentAPI_ISIMM_MP1_GL.Models;

namespace IncidentAPI_ISIMM_MP1_GL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentsDbController : ControllerBase
    {
        private static readonly string[] AllowedSeverities =
        { "LOW", "MEDIUM", "HIGH", "CRITICAL" };
        private static readonly string[] AllowedStatuses =
        { "OPEN", "IN_PROGRESS", "RESOLVED" };
        private readonly IncidentsDbContext _context;

        public IncidentsDbController(IncidentsDbContext context)
        {
            _context = context;
        }

        // GET: api/IncidentsDb
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Incident>>> GetIncidents()
        {
            return await _context.Incidents.ToListAsync();
        }

        // GET: api/IncidentsDb/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Incident>> GetIncident(int id)
        {
            var incident = await _context.Incidents.FindAsync(id);

            if (incident == null)
            {
                return NotFound();
            }

            return incident;
        }

        // PUT: api/IncidentsDb/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIncident(int id, Incident incident)
        {
            if (id != incident.Id)
            {
                return BadRequest();
            }

            _context.Entry(incident).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IncidentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/IncidentsDb
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Incident>> PostIncident(Incident incident)
        {
            if (!AllowedSeverities.Contains(incident.Severity.ToUpper()))
            {
                return BadRequest($"Severity must be one of the following: {string.Join(", ", AllowedSeverities)}");
            }

            incident.Status = "RESOLVED";
            incident.CreatedAt = DateTime.Now;
            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIncident", new { id = incident.Id }, incident);
        }

        // DELETE: api/IncidentsDb/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncident(int id)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident == null)
            {
                return NotFound();
            }

            _context.Incidents.Remove(incident);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IncidentExists(int id)
        {
            return _context.Incidents.Any(e => e.Id == id);
        }
        [HttpGet("getbystatus/{status}")]
        public IActionResult FilterByStatus(string status)
        {
           // var filteredIncidents = _context.Incidents.Where(i => i.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            var incidents = from i in _context.Incidents
                            where i.Status.Contains(status)
                            select i;
            return Ok(incidents);
        }

        [HttpGet("getbystatusasync/{status}")]
        public async Task<IActionResult> FilterByStatusAsync(string status)
        {
            var incidents = await _context.Incidents.Where(i => i.Severity.Contains(status)).ToListAsync();

            return Ok(incidents);
        }

        [HttpGet("getbyseverity/{severity}")]
        public IActionResult FilterBySeverity(string severity)
        {
            // var filteredIncidents = _context.Incidents.Where(i => i.Severity.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            var incidents = from i in _context.Incidents
                            where i.Severity.Contains(severity)
                            select i;
            return Ok(incidents);
        }

        [HttpGet("getbyseverityasync/{severity}")]
        public async Task<IActionResult> FilterBySeverityAsync(string severity)
        {
            var incidents = await _context.Incidents.Where(i => i.Severity.Contains(severity)).ToListAsync();

            return Ok(incidents);
        }
    }
}