using Autofac.Extras.Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Activators;
using Moq;
using Xunit;

namespace Samples.AutofacSamples
{
    /// <summary>
    /// AutofacサイトのGettingStandard
    /// https://autofac.readthedocs.io/en/latest/integration/moq.html
    /// 
    /// </summary>
    public class GettingStandardTest
    {
        //Mock <SystemUnderTest> moq = new Mock<SystemUnderTest>();
        //moq.Setup(x => x.DoWork()).Returns("Used Moq").Verifiable();
        //var underTest = moq.Object;
        //var ret = underTest.DoWork();
        //moq.Verify(x => x.DoWork(), Times.Once);
        //Assert.Equal("Used Moq", ret);

        [Fact]
        public void LooseMockingTest()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var sut = mock.Create<SystemUnderTest>();
            }
        }

        [Fact]
        public void SUT_GetValueTest()
        {
            // AutoMockは　Moqを中では使用しているみたいなので、Moqの書き方でそのまま使えるみたい
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<IDependency>().Setup(x => x.GetValue()).Returns("expected value").Verifiable();
                var sut = mock.Create<SystemUnderTest>();

                // Act
                var actual = sut.DoWork();

                // Assert
                mock.Mock<IDependency>().Verify(x => x.GetValue(), Times.Once);
                Assert.Equal("expected value", actual);
            }
        }

        [Fact]
        public void AutofacRegisterTest()
        {
            var dependency = new DependencyImpl();
            using (var mock = AutoMock.GetLoose(builder => builder.RegisterInstance(dependency).As<IDependency>()))
            {
                // Returns your registered instance.
                // インターフェース実装の場合
                var dep = mock.Create<IDependency>();
                var actual = dep.GetValue();
                Assert.Equal("valueです", actual);

                // If SystemUnderTest depends on IDependency, it will get your dependency instance.
                // コンストラクタインジェクションを使用している場合
                var sut = mock.Create<SystemUnderTest>();
                actual = sut.DoWork();
                Assert.Equal("valueです", actual);
            }
        }

        
    }


    public class SystemUnderTest
    {
        private readonly IDependency _dependency;
        public SystemUnderTest(IDependency dependency)
        {
            _dependency = dependency;
            
        }
        
        public string DoWork()
        {
            return this._dependency.GetValue();
        }
    }

    public class DependencyImpl : IDependency
    {
        public string GetValue()
        {
            return "valueです";
        }
    }

    public interface IDependency
    {
        string GetValue();
    }
}
