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
                return;

            try
            {
                // Crear FirebaseApp usando el archivo firebase.json
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile("firebase.json")
                });

                _initialized = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Error al inicializar Firebase. " +
                    $"Verifique que el archivo firebase.json exista y contenga credenciales válidas. Error: {ex.Message}", ex);
            }
        }
    }
}
