using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Shared.Interfaces
{
    public interface IHasCreationTime
    {
        DateTime CreatedOn { get; set; }
    }
}
