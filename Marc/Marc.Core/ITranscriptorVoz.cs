namespace Marc.Core;

public interface ITranscriptorVoz
{
    Task<string> EscucharYTranscribirAsync();
}