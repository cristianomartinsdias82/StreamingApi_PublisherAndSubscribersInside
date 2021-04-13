using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static EventsPublishingApi.Helpers.ExceptionHelper;

namespace EventsPublishingApi.Results
{
    public class PushStreamResult : IActionResult
    {
        private readonly Action<Stream, CancellationToken> _onStreamAvailable;
        private readonly string _contentType;
        private readonly CancellationToken _requestAborted;

        public PushStreamResult(
            Action<Stream, CancellationToken> onStreamAvailable,
            string contentType,
            CancellationToken requestAborted)
        {
            _onStreamAvailable = onStreamAvailable ?? throw NullArgException(nameof(onStreamAvailable));
            _contentType = contentType ?? throw NullArgException(nameof(contentType));
            _requestAborted = requestAborted;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            var stream = context.HttpContext.Response.Body;

            context.HttpContext.Response.GetTypedHeaders().ContentType = new MediaTypeHeaderValue(_contentType);

            _onStreamAvailable(stream, _requestAborted);

            return Task.CompletedTask;
        }
    }
}
