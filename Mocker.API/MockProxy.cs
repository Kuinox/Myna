using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocker.API
{
    public class MockProxy
    {
        public void Relay(Delegate proxied, object[] args)
        {
            Console.WriteLine($"{proxied.Method.Name} {args.Length}");
        }
    }
}

