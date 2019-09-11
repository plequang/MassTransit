namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Common_Tests;
    using NUnit.Framework;
    using Saga;
    using Scenarios.StateMachines;


    public class Windsor_MultipleSagaStateMachinesFromAssembly :
        Common_SagaStateMachine
    {
        readonly IWindsorContainer _container;

        public Windsor_MultipleSagaStateMachinesFromAssembly()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(x =>
            {
                // The MassTransit.Containers.Tests contains 2 state machine sagas
                x.AddSagaStateMachines(typeof(TestStateMachineSaga).Assembly);
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
