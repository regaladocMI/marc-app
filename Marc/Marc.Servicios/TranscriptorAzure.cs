using Marc.Core;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace Marc.Servicios;

public class TranscriptorAzure : ITranscriptorVoz
{
    public async Task<string> EscucharYTranscribirAsync()
    {
        var configuracion = SpeechConfig.FromSubscription(
            ConfiguracionApp.ObtenerAzureSpeechKey(),
            ConfiguracionApp.ObtenerAzureSpeechRegion());

        configuracion.SetProperty(
            PropertyId.Speech_SegmentationSilenceTimeoutMs,
            (ConfiguracionConversacion.PacienciaSegundos * 1000).ToString());

        var deteccionIdioma = AutoDetectSourceLanguageConfig.FromLanguages(new[] { "en-US", "es-PE" });

        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        using var reconocedor = new SpeechRecognizer(configuracion, deteccionIdioma, audioConfig);


        SpeechRecognitionResult resultado = await reconocedor.RecognizeOnceAsync();

        return resultado.Reason switch
        {
            ResultReason.RecognizedSpeech => resultado.Text,
            ResultReason.NoMatch => string.Empty,
            ResultReason.Canceled => LanzarErrorDeCancelacion(resultado),
            _ => throw new InvalidOperationException($"No se pudo transcribir: {resultado.Reason}")
        };
    }

    private static string LanzarErrorDeCancelacion(SpeechRecognitionResult resultado)
    {
        var detalle = CancellationDetails.FromResult(resultado);
        throw new InvalidOperationException(
            $"Cancelado. Razon: {detalle.Reason}. ErrorCode: {detalle.ErrorCode}. Detalle: {detalle.ErrorDetails}");
    }
}