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
    public class ProdutoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Produto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            return await _context.Produtos.ToListAsync();
        }

        // GET: api/Produto/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            return produto;
        }
        //busca produto
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Produto>>> BuscaProduto(int? id, string descricao = null, int pagina = 1, int itensPorPagina = 10)
        {
            if (pagina <= 0 || itensPorPagina <= 0)
            {
                return BadRequest();
            }

            var produtosConsulta = _context.Produtos.AsQueryable();

            if(id != null)
            {
             produtosConsulta = _context.Produtos.Where(p => p.Id == id);  
            }

            if (!string.IsNullOrEmpty(descricao))
            {
               produtosConsulta = _context.Produtos.Where(p => p.Descricao.Contains(descricao));
            }
            var totalProdutos = await produtosConsulta.CountAsync();

            var produtos = await produtosConsulta.Skip((pagina - 1) * itensPorPagina).Take(itensPorPagina).ToListAsync();

            if (!produtos.Any())
            {
                return NotFound("Nem um produto foi encontrado");
            }
            var metadados = new
            {
                TotalCount = totalProdutos,
                PageSize = itensPorPagina,
                CurrentPage = pagina,
                TotalPages = (int)Math.Ceiling((double)totalProdutos / itensPorPagina)
            };
            Response.Headers.Add("X-Paginacao", JsonConvert.SerializeObject(metadados));
            return Ok(produtos);
        }

        // POST: api/Produto
        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduto", new { id = produto.Id }, produto);
        }

        // PUT: api/Produto/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(int id, Produto produto)
        {
            if (id != produto.Id)
            {
                return BadRequest();
            }

            _context.Entry(produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(id))
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

        // DELETE: api/Produto/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }
    }
}
