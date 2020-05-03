using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Messages;
using MediatR;

namespace Application.Handlers
{
    public class PayloadOnlyHandler : IRequestHandler<PayloadOnlyRequest, PayloadOnlyResponse>
    {
        public Task<PayloadOnlyResponse> Handle(PayloadOnlyRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine("PayloadOnlyHandler");
            return Task.FromResult(new PayloadOnlyResponse() { Success = true});
        }
    }
}
