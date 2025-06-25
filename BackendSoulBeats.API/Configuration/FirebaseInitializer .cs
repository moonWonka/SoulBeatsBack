using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace BackendSoulBeats.API.Configuration
{
    public static class FirebaseInitializer
    {
        private static bool _initialized = false;

        public static void Initialize(string? credentialsPath = null)
        {
            if (_initialized)
                return; // No volver a inicializar si ya fue hecho

            // Intentar usar variable de entorno primero
            var firebaseKeyJson = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_KEY");
            
            AppOptions appOptions;
            
            if (!string.IsNullOrEmpty(firebaseKeyJson))
            {
                // Usar JSON desde variable de entorno (desarrollo y producción)
                appOptions = new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(firebaseKeyJson)
                };
            }
            else if (!string.IsNullOrEmpty(credentialsPath))
            {
                // Fallback para archivo específico si se proporciona
                if (!File.Exists(credentialsPath))
                    throw new FileNotFoundException($"El archivo de credenciales de Firebase no se encontró en la ruta especificada: {credentialsPath}");

                appOptions = new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(credentialsPath)
                };
            }
            else
            {
                throw new InvalidOperationException(
                    "No se encontraron credenciales de Firebase. " +
                    "Configure la variable de entorno FIREBASE_SERVICE_ACCOUNT_KEY o proporcione la ruta del archivo de credenciales.");
            }

            FirebaseApp.Create(appOptions);
            _initialized = true;
        }
    }
}
