using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MyAppAPI.Data;
using MyAppAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace MyAppAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PedidoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedido()
        {
            return await _context.Pedidos
                                 .Include(p => p.Cliente)
                                 .Include(p => p.Produto)
                                 .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedidoId(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Produto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }
            return Ok(pedido);
        }
        //busca pedido 
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Pedido>>> BuscaPedido(int id, int pagina = 1, int itensPorPagina = 10)
        {
            if (id <= 0)
            {
                return BadRequest("Informe um pedido Valido !");
            }
            var pedidosConsulta =  _context.Pedidos.Where(p => p.Id == id);
            var totalPedidos = await pedidosConsulta.CountAsync();
            var pedidos = await pedidosConsulta.Skip((pagina - 1) * itensPorPagina).Take(itensPorPagina).ToListAsync();

            if (!pedidos.Any())
            {
                return NotFound("Nem um item correspondente");
            }

            var metadados = new
            {
                TotalCount = totalPedidos,
                PageSize = itensPorPagina,
                CurrentPage = pagina,
                TotalPages = (int)Math.Ceiling((double)totalPedidos / itensPorPagina)
            };

            Response.Headers.Add("X-Paginacao", JsonConvert.SerializeObject(metadados));

            return Ok(pedidos);
        }

        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPedidoId", new { id = pedido.Id }, pedido);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(int id, Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return BadRequest();
            }

            _context.Entry(pedido).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoExiste(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PedidoExiste(int id)
        {
            return _context.Pedidos.Any(p => p.Id == id);
        }
    }
}
