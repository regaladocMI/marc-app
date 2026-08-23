namespace Marc.Core
{
    public class Tema
    {
        public int IdTema { get; set; }
        public int IdUsuario { get; set; }
        public int IdNivelIngles { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string PromptBase { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
