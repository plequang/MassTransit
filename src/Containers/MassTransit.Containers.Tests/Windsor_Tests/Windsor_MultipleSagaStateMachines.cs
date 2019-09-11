namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Common_Tests;
    using MassTransit.Containers.Tests.Common_Tests.Discovery;
    using NUnit.Framework;
    using Saga;
    using Scenarios.StateMachines;


    public class Windsor_MultipleSagaStateMachines :
        Common_SagaStateMachine
    {
        readonly IWindsorContainer _container;

        public Windsor_MultipleSagaStateMachines()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<TestStateMachineSaga, TestInstance>();
                x.AddSagaStateMachine<DiscoveryPingStateMachine, DiscoveryPingState>();
                x.AddBus(provider => BusControl);
            });

            _container.Register(Component.For<PublishTestStartedActivity>().LifestyleScoped(),
                Component.For(typeof(ISagaRepository<>)).ImplementedBy(typeof(InMemorySagaRepository<>)).LifestyleSingleton());
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureSagaStateMachine(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSagas(_container);
        }
    }
}
