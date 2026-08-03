using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Utilities.Encryption.Interface
{
    public interface IEncryptionHandler
    {
        public Task<string> HashString(string input);
        public Task<string> EncryptString(string input);
        public Task<string> DecryptString(string input);
    }
}
