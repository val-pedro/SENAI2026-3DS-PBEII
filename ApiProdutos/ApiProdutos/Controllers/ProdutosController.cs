using ApiProdutos.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiProdutos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        // Lista em mémória
        private static List<Produto> produtos = new List<Produto>
        {
            new Produto { Id = 1, Nome = "Mouse", Preco = 50},

            new Produto { Id = 2, Nome = "Teclado", Preco = 120},

            new Produto { Id = 3, Nome = "Monitor", Preco = 900},
        };

        // GET: api/produtos
        [HttpGet]
        public ActionResult<List<Produto>> buscarTodosProdutos() 
        {
            return Ok(produtos);
        }

        // GET: api/produtos/1
        [HttpGet("{id}")]
        public ActionResult<Produto> BuscarPorId(int id)
        {
            var prod = produtos.FirstOrDefault(p => p.Id == id);

            if(prod == null)
            {
                return NotFound("Produto não encontrado");
            }

            return Ok(prod);
        }

        // POST: api/produtos
        [HttpPost]
        public ActionResult CriarProduto(Produto produto)
        {
            produtos.Add(produto);

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id = produto.Id },
                produto
            );
        }

        // PUT: api/produtos
        [HttpPut("{id}")]
        public ActionResult Atualizar(int id, Produto produto)
        {
            var prod = produtos.FirstOrDefault(p => p.Id == id);

            if(prod == null)
            {
                return NotFound("Produto não encontrado");
            }

            prod.Nome = produto.Nome;
            prod.Preco = produto.Preco;

            return NoContent();
        }

        // DELETE: api/produtos/1
        [HttpDelete("{id}")]
        public ActionResult Deletar(int id)
        {
            var prod = produtos.FirstOrDefault(p => p.Id == id);

            if(prod == null)
            {
                return NotFound("Produto não encontrado");
            }

            produtos.Remove(prod);
            return NoContent();
        }

    }
}
