using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperLibrary.Cryptography
{
    public enum KeySize : int
    {
        SIZE_512 = 512,
        SIZE_1024 = 1024,
        SIZE_2048 = 2048,
        SIZE_4096 = 4096,
        SIZE_8192 = 8192,
        SIZE_16384 = 16384
    }
}
