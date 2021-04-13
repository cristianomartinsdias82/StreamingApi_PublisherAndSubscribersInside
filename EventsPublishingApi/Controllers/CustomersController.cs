using EventsPublishingApi.Core;
using EventsPublishingApi.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventsPublishingApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private static ConcurrentBag<StreamWriter> _clients;

        static CustomersController()
        {
            _clients = new ConcurrentBag<StreamWriter>();
        }

        [HttpGet("{id:int}")]
        public async Task<Customer> Get(int id, CancellationToken cancellationToken)
        {
            await Task.Delay(0, cancellationToken);

            return new Customer();
        }

        [HttpPost]
        public async Task<IActionResult> Post(Customer customer, CancellationToken cancellationToken)
        {
            ++customer.Id;

            await DispatchEvent(customer, ApplicationEvents.CustomerRegistration);

            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, Customer customer, CancellationToken cancellationToken)
        {
            await DispatchEvent(customer, ApplicationEvents.CustomerDataUpdate);

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await DispatchEvent(new { id }, ApplicationEvents.CustomerDeletion);

            return StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("streaming")]
        public IActionResult Stream()
        => new PushStreamResult(OnStreamAvailable, "text/event-stream", HttpContext.RequestAborted);

        private void OnStreamAvailable(Stream stream, CancellationToken requestAborted)
        {
            var wait = requestAborted.WaitHandle;

            _clients.Add(new StreamWriter(stream));

            wait.WaitOne();

            _clients.TryTake(out StreamWriter ignore);
        }

        private static async Task DispatchEvent(object data, ApplicationEvents applicationEvent)
        {
            foreach (var client in _clients ?? new ConcurrentBag<StreamWriter>())
            {
                string eventData = string.Format("{0}\n", JsonConvert.SerializeObject(new { data, applicationEvent = $"{applicationEvent}" }));
                await client.WriteAsync(eventData);
                await client.FlushAsync();
            }
        }
    }
}
