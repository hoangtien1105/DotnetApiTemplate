using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels.ResponseModels
{
    public class ResponseGenericModel <TEntity>
    {
        public bool Status { get; set; } = false;
        public string Message { get; set; } = "";
        public TEntity? Data { get; set; } 

    }
}
