using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventsPublishingApi.Helpers
{
    public static class ExceptionHelper
    {
        public static ArgumentNullException NullArgException(string argumentName)
        => throw new ArgumentNullException(argumentName);
    }
}
