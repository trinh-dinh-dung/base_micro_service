using DocumentFormat.OpenXml.Wordprocessing;
using Application.Common.Status;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Status
{

    public enum BusinessServiceTypeSendRabbitMq
    {
        SopService = 1,
        MaintainanceManagementService = 2
    }
}

