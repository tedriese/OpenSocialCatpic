// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICryptoService.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines the ICryptoService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Utils.OAuth
{
    /// <summary>
    /// Defines crypto service behavior
    /// </summary>
    public interface ICryptoService
    {
        /// <summary>
        /// Encrypts data string
        /// </summary>
        /// <param name="data"> Data string. </param>
        /// <returns> Encrypted byte array </returns>
        byte[] Encrypt(string data);

        /// <summary>
        /// Decrypts data string
        /// </summary>
        /// <param name="data"> Encrypted data array. </param>
        /// <returns> Target string</returns>
        string Decrypt(byte[] data);
    }
}