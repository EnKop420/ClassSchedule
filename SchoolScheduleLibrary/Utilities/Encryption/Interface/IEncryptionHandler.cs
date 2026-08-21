using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Utilities.Encryption.Interface
{
    /// <summary>
    /// Provides high level functions for string hashing, encryption, and decryption.
    /// 
    /// Used mainly by the different Services for sensitive data.
    /// </summary>
    public interface IEncryptionHandler
    {
        /// <summary>
        /// Decrypts a string input with the EncryptionKey.
        /// </summary>
        /// <remarks>
        /// Uses AES (Advanced Encryption Standard), a symmetric key encryption algorithm.
        /// </remarks>
        /// <param name="input"></param>
        /// <returns>The decrypted string as a raw string.</returns>
        public Task<string> DecryptString(string input);

        /// <summary>
        /// Encrypts a string using the EncryptionKey.
        /// </summary>
        /// <remarks>
        /// Uses AES (Advanced Encryption Standard), a symmetric key encryption algorithm.
        /// </remarks>
        /// <param name="input"></param>
        /// <returns>A string converted version of the encrypted bytes</returns>
        public Task<string> EncryptString(string input);

        /// <summary>
        /// Hashes a string using SHA256 algorithm as it has not been "cracked" yet.
        /// </summary>
        /// <remarks>
        /// Adds a additional SALT to make the text even more unique and harder to guess.
        /// </remarks>
        /// <param name="input"></param>
        /// <returns>A string converted version of the hashed bytes</returns>
        public Task<string> HashString(string input);
    }
}
