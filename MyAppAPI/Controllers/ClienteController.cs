using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAppAPI.Data;
using MyAppAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/cliente
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetCliente()
        {
            return await _context.Clientes.ToListAsync();
        }

        //busca cliente
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Cliente>>> BuscaCliente(string nome = null, int pagina = 1, int itensPorPagina = 10)
        {
            if(pagina <= 0 || itensPorPagina <= 0)
            {
                return BadRequest();
            }

            var clientesConsulta = _context.Clientes.AsQueryable();

            if(!string.IsNullOrEmpty(nome))
            {
                clientesConsulta = _context.Clientes.Where(c => c.Nome.Contains(nome));
            }
            var totalClientes = await clientesConsulta.CountAsync();

            var clientes =  await clientesConsulta.Skip((pagina - 1) * itensPorPagina).Take(itensPorPagina).ToListAsync();

            if(!clientes.Any())
            {
                return NotFound("Nem um cliente encontrado");
            }
            var metadados = new
            {
                TotalCount = totalClientes,
                PageSize = itensPorPagina,
                CurrentPage = pagina,
                TotalPages = (int)Math.Ceiling((double)totalClientes / itensPorPagina)
            };
            Response.Headers.Add("X-Paginacao", JsonConvert.SerializeObject(metadados));
            return Ok(clientes);
        }
        // GET: api/cliente id
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        // POST: api/cliente
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCliente", new { id = cliente.Id }, cliente);
        }

        // PUT: api/cliente/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest();
            }

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExistente(id))
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

        // DELETE: api/cliente/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClienteExistente(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
