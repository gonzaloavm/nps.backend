using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common
{
    public class BusinessErrorCodes
    {
        /// <summary> Error generico </summary>
        public const string Generic = "ERROR_GENERIC";

        /// <summary> Credenciales inválidas. </summary>
        public const string InvalidCredentials = "INVALID_CREDENTIALS";

        /// <summary> Cuenta bloqueada por multiples intentos. </summary>
        public const string AccountLocked = "ACCOUNT_LOCKED";

        /// <summary> El recurso solicitado no existe. </summary>
        public const string NotFound = "NOT_FOUND";

        /// <summary> La sesión ha expirado por inactividad o tiempo de token. </summary>
        public const string SessionExpired = "SESSION_EXPIRED";

        /// <summary> Se ha detectado un uso malintencionado o duplicado de tokens. </summary>
        public const string SecurityBreach = "SECURITY_BREACH";

        /// <summary> El usuario ya realizó la acción (ej. votar) y no puede repetir. </summary>
        public const string AlreadyProcessed = "ALREADY_PROCESSED";

        /// <summary> Conflicto con datos existentes (ej. nombre de usuario duplicado). </summary>
        public const string DuplicateResource = "DUPLICATE_RESOURCE";

        /// <summary> Sin código de error asignado. </summary>
        public const string None = "NONE";
    }
}
