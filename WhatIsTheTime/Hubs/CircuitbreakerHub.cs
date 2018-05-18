using Microsoft.AspNet.SignalR;

namespace WhatIsTheTime.Hubs
{
    public class CircuitbreakerHub : Hub
    {
        public void Send(string name, string state)
        {
            Clients.All.circuitstatechange(name, state);
        }
    }
}