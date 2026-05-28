using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoList.Services;
using ToDoList.Models.DTOs.ComentarioDto;
using System;

namespace ToDoList.Controllers
{
    [ApiController]
    [Route("api/tarefas/{tarefaId}/comentarios")]
    //[Authorize]
    public class ComentarioController : ControllerBase
    {
        private readonly ComentarioService _service;

        public ComentarioController(ComentarioService service)
        {
            _service = service;
        }

        // GET: api/tarefas/{tarefaId}/comentarios
        [HttpGet]
        public async Task<IActionResult> GetAll(Guid tarefaId)
        {
            var comentarios = await _service.ListarPorTarefaAsync(tarefaId);
            return Ok(comentarios);
        }

        // POST: api/tarefas/{tarefaId}/comentarios
        [HttpPost]
        public async Task<IActionResult> Create(Guid tarefaId, [FromBody] ComentarioCreateDto dto)
        {
            if (!TryGetUsuarioId(out var usuarioId))
                return Unauthorized();

            var comentario = await _service.CriarAsync(dto, tarefaId, usuarioId);

            return Created("", comentario);
        }

        // DELETE: api/tarefas/{tarefaId}/comentarios/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid tarefaId, Guid id)
        {
            if (!TryGetUsuarioId(out var usuarioId))
                return Unauthorized();

            var result = await _service.DeletarAsync(tarefaId, id, usuarioId);

            if (!result)
            {
                // Regra: se falhou, retornar 403 (não 404)
                return Forbid();
            }

            return NoContent(); // 204
        }

        // Método auxiliar
        private bool TryGetUsuarioId(out Guid usuarioId)
        {
            usuarioId = Guid.Empty;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return false;

            return Guid.TryParse(userId, out usuarioId);
        }
    }
}