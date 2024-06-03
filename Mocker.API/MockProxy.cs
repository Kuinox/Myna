using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocker.API
{
    public class MockProxy(Func<Delegate, object[], object> relay)
    {
        public object Relay(Delegate proxied, object[] args) => relay(proxied, args);
    }
}

