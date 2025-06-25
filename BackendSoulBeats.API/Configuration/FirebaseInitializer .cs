using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Text;

namespace BackendSoulBeats.API.Configuration
{
    public static class FirebaseInitializer
    {
        private static bool _initialized = false;

        public static void Initialize()
        {
            if (_initialized)
                return;

            // Obtener la variable base64 desde el entorno
            var firebaseKeyBase64 = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_KEY_B64");

            if (string.IsNullOrWhiteSpace(firebaseKeyBase64))
            {
                throw new InvalidOperationException(
                    "No se encontró la variable FIREBASE_SERVICE_ACCOUNT_KEY_B64 en el entorno.");
            }

            string firebaseJson;

            try
            {
                // Decodificar desde base64 a string JSON
                byte[] data = Convert.FromBase64String(firebaseKeyBase64);
                firebaseJson = Encoding.UTF8.GetString(data);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al decodificar la clave de Firebase desde base64.", ex);
            }

            try
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile("firebase.json")
                });

                _initialized = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al inicializar FirebaseApp con las credenciales proporcionadas.", ex);
            }
        }
    }
}
