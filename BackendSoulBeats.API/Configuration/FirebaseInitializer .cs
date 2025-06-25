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
                return; // No volver a inicializar si ya fue hecho            // Intentar usar variable de entorno primero
            var firebaseKeyJson = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_KEY");
            
            // Validación estricta: fallar si la variable está vacía o solo contiene espacios
            if (string.IsNullOrWhiteSpace(firebaseKeyJson) && string.IsNullOrWhiteSpace(credentialsPath))
            {
                throw new InvalidOperationException(
                    "CONFIGURACIÓN REQUERIDA: No se encontraron credenciales de Firebase. " +
                    "El build debe fallar para evitar problemas en runtime. " +
                    "Configure la variable de entorno FIREBASE_SERVICE_ACCOUNT_KEY o proporcione la ruta del archivo de credenciales.");
            }            
            AppOptions appOptions;
            
            if (!string.IsNullOrWhiteSpace(firebaseKeyJson))
            {
                // Validar que el JSON tenga contenido válido
                if (firebaseKeyJson.Trim().Length < 50) // Un JSON válido debe tener al menos 50 caracteres
                {
                    throw new InvalidOperationException(
                        "CONFIGURACIÓN INVÁLIDA: La variable FIREBASE_SERVICE_ACCOUNT_KEY contiene un valor que parece ser inválido. " +
                        "Debe contener un JSON completo de credenciales de Firebase.");
                }

                // Usar JSON desde variable de entorno (desarrollo y producción)
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
            }
            else if (!string.IsNullOrWhiteSpace(credentialsPath))
            {
                // Fallback para archivo específico si se proporciona
                if (!File.Exists(credentialsPath))
                    throw new FileNotFoundException(
                        $"CONFIGURACIÓN INVÁLIDA: El archivo de credenciales de Firebase no se encontró en la ruta especificada: {credentialsPath}");

                try
                {
                    appOptions = new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(credentialsPath)
                    };
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"CONFIGURACIÓN INVÁLIDA: Error al cargar las credenciales de Firebase desde el archivo: {credentialsPath}. " +
                        $"Error: {ex.Message}", ex);
                }
            }
            else
            {
                // Este caso ya no debería ocurrir debido a la validación anterior
                throw new InvalidOperationException(
                    "ERROR INTERNO: No se encontraron credenciales de Firebase después de la validación inicial.");
            }

            FirebaseApp.Create(appOptions);
            _initialized = true;
        }
    }
}
