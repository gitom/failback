namespace WhatIsTheTime.Models
{
    public class CurrentTime : IProvider
    {
        public string Now { get; set; }
        public string Provider { get; set; }
    }

    public interface IProvider
    {
        string Provider { get; set; }
    }
}