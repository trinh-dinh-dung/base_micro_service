using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices.DataConfig
{
    public interface IDataConfig
    {
        string GetConnectStringByConnectName(string key);
        Task<string> GetUserIDByUserName(string UserName);
    }
}
