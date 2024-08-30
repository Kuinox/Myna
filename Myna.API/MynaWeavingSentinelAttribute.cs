using System;

namespace Myna.API
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MynaWeavingSentinelAttribute : Attribute
    {
        public MynaWeavingSentinelAttribute() : base()
        {
            
        }
    }
}
