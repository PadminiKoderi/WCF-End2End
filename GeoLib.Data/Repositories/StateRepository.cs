using System;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;
using GeoLib.Core;

namespace GeoLib.Data
{
    public class StateRepository : IStateRepository
    {
        

        public State Get(string abbrev)
        {
            //using (GeoLibDbContext entityContext = new GeoLibDbContext())
            //{
            //    return entityContext.StateSet.FirstOrDefault(e => e.Abbreviation.ToUpper() == abbrev.ToUpper());
            //}
            State x = new State();
            return x;
        }

        public IEnumerable<State> Get(bool primaryOnly)
        {
            //using (GeoLibDbContext entityContext = new GeoLibDbContext())
            //{
            //    return entityContext.StateSet.Where(e => e.IsPrimaryState == primaryOnly).ToFullyLoaded();
            //}
            return null;
        }
    }
}
