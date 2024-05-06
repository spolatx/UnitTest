using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Services
{
    public interface IIdentityValidator
    {
        bool IsValid(string identityNumber);
        // bool CheckConnectionToRemoteServer();
        string Country {  get; }
        public ValidationMode ValidationMode { get; set; }
       
    }
    public enum ValidationMode
    {
        Detailed,
        Quick
    }
}
