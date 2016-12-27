using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GeoLib.Core;


namespace GeoLib.Data
{
    public interface IStateRepository 
    {
        State Get(string abbrev);
        IEnumerable<State> Get(bool primaryOnly);
    }
}
