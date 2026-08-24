using Marc.Core;
using Microsoft.CognitiveServices.Speech;

namespace Marc.Servicios;

public class SintetizadorAzure : ISintetizadorVoz
{
    public async Task ReproducirAsync(string texto)
    {
        var configuracion = SpeechConfig.FromSubscription(
            ConfiguracionApp.ObtenerAzureSpeechKey(),
            ConfiguracionApp.ObtenerAzureSpeechRegion());

        configuracion.SpeechSynthesisVoiceName = "en-US-AvaMultilingualNeural";

        using var sintetizador = new SpeechSynthesizer(configuracion);
        SpeechSynthesisResult resultado = await sintetizador.SpeakTextAsync(texto);

        if (resultado.Reason == ResultReason.Canceled)
        {
            var detalle = SpeechSynthesisCancellationDetails.FromResult(resultado);
            throw new InvalidOperationException(
                $"No se pudo generar la voz. Razon: {detalle.Reason}. Detalle: {detalle.ErrorDetails}");
        }
    }
}