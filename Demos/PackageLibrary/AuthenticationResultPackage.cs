﻿using HelperLibrary.Networking.ClientServer.Packages;
using System;

namespace PackageLibrary
{
    [Serializable]
    public class AuthenticationResultPackage : BasePackage
    {
        public readonly AuthenticationResult Result;

        public AuthenticationResultPackage(AuthenticationResult result, string senderUid, string destinationUid) : base(senderUid, destinationUid)
        {
            Result = result;
        }
    }
}
