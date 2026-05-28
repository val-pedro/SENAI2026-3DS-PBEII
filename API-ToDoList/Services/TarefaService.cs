using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models.DTOs.TarefaDto;

namespace ToDoList.Services;

public class TarefaService
{
    // Contexto do banco de dados (Entity Framework)
    private readonly AppDbContext _context;

    // Construtor com injeção de dependência do DbContext
    public TarefaService(AppDbContext context)
    {
        _context = context;
    }

    // Lista todas as tarefas de um usuário específico
    public async Task<List<TarefaResponseDto>> ListarAsync(Guid usuarioId)
    {
        return await _context.Tarefas
            .Where(t => t.UsuarioId == usuarioId) // Filtra pelo usuário
            .Select(t => t.ToResponse()) // Converte entidade para DTO
            .ToListAsync(); // Executa a query de forma assíncrona
    }

    // Busca uma tarefa pelo ID, garantindo que pertence ao usuário
    public async Task<TarefaResponseDto?> BuscarPorIdAsync(Guid id, Guid usuarioId)
    {
        var tarefa = await _context.Tarefas
            .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

        // Retorna a tarefa convertida ou null se não encontrar
        return tarefa?.ToResponse();
    }

    // Cria uma nova tarefa
    public async Task<TarefaResponseDto> CriarAsync(TarefaCreateDto dto, Guid usuarioId)
    {
        // Converte DTO em entidade
        var tarefa = dto.ToEntity(usuarioId);

        // Adiciona no banco
        _context.Tarefas.Add(tarefa);
        await _context.SaveChangesAsync(); // Salva alterações

        // Retorna a tarefa criada como DTO
        return tarefa.ToResponse();
    }

    // Atualiza uma tarefa existente
    public async Task<TarefaResponseDto?> AtualizarAsync(Guid id, TarefaUpdateDto dto, Guid usuarioId)
    {
        // Busca a tarefa pelo ID e usuário
        var tarefa = await _context.Tarefas
            .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

        if (tarefa is null) return null; // Se não encontrar, retorna null

        // Atualiza os campos da tarefa
        tarefa.Titulo = dto.Titulo.Trim(); // Remove espaços extras
        tarefa.Descricao = dto.Descricao?.Trim();
        tarefa.Concluida = dto.Concluida;
        tarefa.AtualizadaEm = DateTime.UtcNow; // Atualiza data

        await _context.SaveChangesAsync(); // Salva alterações

        return tarefa.ToResponse();
    }

    // Deleta uma tarefa
    public async Task<bool> DeletarAsync(Guid id, Guid usuarioId)
    {
        // Busca a tarefa pelo ID e usuário
        var tarefa = await _context.Tarefas
            .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

        if (tarefa is null) return false; // Retorna false se não encontrar

        _context.Tarefas.Remove(tarefa); // Remove do banco
        await _context.SaveChangesAsync(); // Salva alterações

        return true; // Retorna sucesso
    }
}