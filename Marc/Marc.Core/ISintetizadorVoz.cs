namespace Marc.Core;

public interface ISintetizadorVoz
{
    Task ReproducirAsync(string texto, Action<string>? alPronunciarPalabra = null);
}