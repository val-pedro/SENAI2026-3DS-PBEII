namespace ToDoList.Models.Entities
{
    public class Comentario
    {
        public Guid Id { get; set; }
        public string Conteudo { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        // FK - 1 Tarefa pode ter N Comentarios
        public Guid TarefaId { get; set; }
        public Tarefa? Tarefa { get; set; }

        // FK - 1 Usuario pode ter N Comentarios
        public Guid UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}