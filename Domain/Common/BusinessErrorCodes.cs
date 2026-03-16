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

        /// <summary> Sin código de error asignado. </summary>
        public const string None = "NONE";
    }
}
