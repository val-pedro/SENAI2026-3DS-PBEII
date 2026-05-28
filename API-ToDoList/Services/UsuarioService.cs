using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models.DTOs.UsuarioDto;

namespace ToDoList.Services
{
    public class UsuarioService(AppDbContext context)
    {
        // Retorna todos os usuários cadastrados, sem rastreamento de mudanças (somente leitura)
        public async Task<List<UsuarioResponseDto>> GetAllAsync()
        {
            var usuarios = await context.Usuarios
                //.Include(u => u.Tarefas) // Descomente para incluir as tarefas do usuário na resposta
                .AsNoTracking()
                .ToListAsync();

            // Converte cada entidade para o DTO de resposta antes de retornar
            return usuarios.Select(u => u.ToResponse()).ToList();
        }

        // Busca um usuário específico pelo seu ID
        public async Task<UsuarioResponseDto> GetByIdAsync(Guid id)
        {
            var usuario = await context.Usuarios
                //.Include(u => u.Tarefas) // Descomente para incluir as tarefas do usuário na resposta
                .FirstOrDefaultAsync(u => u.Id == id); // Retorna null caso o usuário não seja encontrado

            // O operador ?. garante que retorna null sem lançar exceção caso o usuário não exista
            return usuario?.ToResponse();
        }

        // Cria um novo usuário a partir dos dados recebidos no DTO
        public async Task<UsuarioResponseDto> CreateAsync(UsuarioCreateDto dto)
        {
            // Verifica se já existe um usuário cadastrado com o mesmo e-mail
            var emailExiste = await context.Usuarios
                .AnyAsync(u => u.Email == dto.Email);

            if (emailExiste)
            {
                // Lança exceção para impedir cadastro duplicado por e-mail
                throw new Exception("Este e-mail já está cadastrado.");
            }

            // Converte o DTO de criação para a entidade do banco de dados
            var usuario = dto.ToEntity();

            // Gera o hash da senha antes de persistir — nunca armazene senhas em texto puro
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);

            // Adiciona o novo usuário ao contexto e salva no banco de dados
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            // Retorna o usuário criado já no formato de resposta (DTO)
            return usuario.ToResponse();
        }
    }
}