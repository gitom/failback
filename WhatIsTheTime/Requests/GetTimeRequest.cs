using MediatR;
using WhatIsTheTime.Models;

namespace WhatIsTheTime.Requests
{
    public class GetTimeRequest : IRequest<CurrentTime> { }
}