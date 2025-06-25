using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace BackendSoulBeats.API.Configuration
{
    public static class FirebaseInitializer
    {
        private static bool _initialized = false;
        public static void Initialize()
        {
            if (_initialized)
                return; // No volver a inicializar si ya fue hecho

            // Intentar usar variable de entorno
            var firebaseKeyJson = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_KEY");

            if (string.IsNullOrWhiteSpace(firebaseKeyJson))
            {
                throw new InvalidOperationException(
                    "No se encontraron credenciales de Firebase. " +
                    "Configure la variable de entorno FIREBASE_SERVICE_ACCOUNT_KEY.");
            }

            AppOptions appOptions;

            // Usar JSON desde variable de entorno
            try
            {
                appOptions = new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(firebaseKeyJson)
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "CONFIGURACIÓN INVÁLIDA: Error al procesar las credenciales de Firebase desde la variable de entorno. " +
                    $"Verifique que el JSON sea válido. Error: {ex.Message}", ex);
            }

            FirebaseApp.Create(appOptions);
            _initialized = true;
        }
    }
}
