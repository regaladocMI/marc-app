using Marc.Core;
using Microsoft.CognitiveServices.Speech;

namespace Marc.Servicios;

public class SintetizadorAzure : ISintetizadorVoz
{
    public async Task ReproducirAsync(string texto, Action<string>? alPronunciarPalabra = null)
    {
        var configuracion = SpeechConfig.FromSubscription(
            ConfiguracionApp.ObtenerAzureSpeechKey(),
            ConfiguracionApp.ObtenerAzureSpeechRegion());

        configuracion.SpeechSynthesisVoiceName = "en-US-AvaMultilingualNeural";

        using var sintetizador = new SpeechSynthesizer(configuracion);

        if (alPronunciarPalabra is not null)
        {
            sintetizador.WordBoundary += (sender, evento) =>
            {
                alPronunciarPalabra(evento.Text);
            };
        }

        int porcentajeVelocidad = (int)((ConfiguracionConversacion.VelocidadHabla - 1.0) * 100);
        string ssml = $"""
        <speak version="1.0" xmlns="http://www.w3.org/2001/10/synthesis" xml:lang="en-US">
          <voice name="en-US-AvaMultilingualNeural">
            <prosody rate="{porcentajeVelocidad}%">{System.Security.SecurityElement.Escape(texto)}</prosody>
          </voice>
        </speak>
        """;

        SpeechSynthesisResult resultado = await sintetizador.SpeakSsmlAsync(ssml);

        if (resultado.Reason == ResultReason.Canceled)
        {
            var detalle = SpeechSynthesisCancellationDetails.FromResult(resultado);
            throw new InvalidOperationException(
                $"No se pudo generar la voz. Razon: {detalle.Reason}. Detalle: {detalle.ErrorDetails}");
        }
    }
}