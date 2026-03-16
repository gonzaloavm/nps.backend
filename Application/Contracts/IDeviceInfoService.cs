using Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts
{
    public interface IDeviceInfoService : IServiceBase
    {
        string GetDeviceName();
        string GetIpAddress();
    }
}
