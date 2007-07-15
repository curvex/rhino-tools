using MbUnit.Framework;

namespace Rhino.Testing.Tests.AutoMocking
{
    [TestFixture]
    public class AutoMockingContainerTests : AutoMockingTests
    {
        [Test]
        public void Resolving_ConditionalDependencyWithMockMissing_WillNotResolve()
        {
            _container.MarkMissing<ICollectionOfServices>();
            ComponentBeingConfigured target = _container.Create<ComponentBeingConfigured>();
            Assert.IsNull(target.Services);
        }

        [Test]
        public void Resolving_WithComponent_ReturnsMock()
        {
            ComponentBeingConfigured target = _container.Create<ComponentBeingConfigured>();

            using (_mocks.Unordered())
            {
                target.ReallyCoolService.SayHello();
            }

            _mocks.ReplayAll();
            target.ReallyCoolService.SayHello();
            _mocks.VerifyAll();

            _container.Dispose();
        }

        [Test]
        public void Resolving_WithOtherImplementation_ReturnsMock()
        {
            _container.AddComponent("DefaultCollectionOfServices", typeof (ICollectionOfServices),
                                    typeof (DefaultCollectionOfServices));
            ComponentBeingConfigured target = _container.Create<ComponentBeingConfigured>();

            _mocks.ReplayAll();
            Assert.IsInstanceOfType(typeof (DefaultCollectionOfServices), target.Services);
            _mocks.VerifyAll();

            _container.Dispose();
        }

        [Test]
        public void Resolving_WithComponentWithStub_ReturnsMock()
        {
            _container.Mark<ICollectionOfServices>().Stubbed();
            ComponentBeingConfigured target = _container.Create<ComponentBeingConfigured>();

            using (_mocks.Unordered())
            {
                target.Services.SomethingToDispose.Dispose();
            }

            _mocks.ReplayAll();
            target.RunDispose();
            _mocks.VerifyAll();

            _container.Dispose();
        }

        [Test]
        public void Resolving_GetTwice_ReturnsSameMock()
        {
            _container.Mark<ICollectionOfServices>().Stubbed();
            ComponentBeingConfigured target1 = _container.Create<ComponentBeingConfigured>();
            ComponentBeingConfigured target2 = _container.Resolve<ComponentBeingConfigured>();

            Assert.AreEqual(target1, target2);
            Assert.AreEqual(target1.ReallyCoolService, target2.ReallyCoolService);
        }

        [Test]
        public void Get_NotGotten_ReturnsMock()
        {
            Assert.IsNotNull(_container.Get<IReallyCoolService>());
        }

        [Test]
        public void Get_AlreadyGotten_ReturnsMock()
        {
            ComponentBeingConfigured target = _container.Create<ComponentBeingConfigured>();
            Assert.AreEqual(target.ReallyCoolService, _container.Get<IReallyCoolService>());
        }
    }
}