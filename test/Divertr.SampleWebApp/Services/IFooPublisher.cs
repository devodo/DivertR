using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Divertr.SampleWebApp.Model;

namespace Divertr.SampleWebApp.Services
{
    public interface IFooPublisher
    {
        Task Publish(FooEvent fooEvent);
    }
}