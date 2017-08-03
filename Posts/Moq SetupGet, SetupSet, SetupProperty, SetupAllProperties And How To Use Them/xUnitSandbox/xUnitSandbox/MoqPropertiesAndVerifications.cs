using System;
using FluentAssertions;
using Moq;
using Xunit;

namespace xUnitSandbox
{
    public interface IPropertyManager
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        void MutateFirstName(string name);
    }

    public class PropertyManager : IPropertyManager
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public void MutateFirstName(string name)
        {
            this.FirstName = name;
        }
    }

    class PropertyManagerConsumer
    {
        private readonly IPropertyManager _propertyManager;

        public PropertyManagerConsumer(IPropertyManager propertyManager)
        {
            _propertyManager = propertyManager;
        }

        public void ChangeName(string name)
        {
            _propertyManager.FirstName = name;
        }

        public string GetName()
        {
            return _propertyManager.FirstName;
        }

        public void ChangeRemoteName(string name)
        {
            _propertyManager.MutateFirstName(name);
        }
    }

    public class MoqPropertiesAndVerifications
    {
        [Fact]
        public void Verify()
        {
            var mock = new Mock<IPropertyManager>();
            var nameUser = new PropertyManagerConsumer(mock.Object);

            nameUser.ChangeRemoteName("My dear old wig");

            //we are verifying that ChangeRemoteName sends the correct string to MutateFirstName
            mock.Verify(m => m.MutateFirstName(It.Is<string>(a => a == "My dear old wig")), Times.Once);
        }

        [Fact]
        public void ItIsAny()
        {
            var mock = new Mock<IPropertyManager>();
            var nameUser = new PropertyManagerConsumer(mock.Object);

            nameUser.ChangeRemoteName("Hamid");

            mock.Verify(m => m.MutateFirstName(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void VerifyPropertyWithVerifyGet()
        {
            var mock = new Mock<IPropertyManager>();
            var nameUser = new PropertyManagerConsumer(mock.Object);

            nameUser.GetName();

            mock.VerifyGet(m => m.FirstName, Times.Once);
        }

        [Fact]
        public void VerifyPropertyIsSet_WithSpecificValue_WithVerifySet()
        {
            var mock = new Mock<IPropertyManager>();
            var nameUser = new PropertyManagerConsumer(mock.Object);

            nameUser.ChangeName("No Shrubbery!");

            mock.VerifySet(m => m.FirstName = "No Shrubbery!");
        }

        [Fact]
        public void StupPropertyWithSetupGet()
        {
            var mock = new Mock<IPropertyManager>();
            var nameUser = new PropertyManagerConsumer(mock.Object);
            mock.SetupGet(m => m.FirstName).Returns("Shrubbery!");

            var name = nameUser.GetName();

            name.Should().Be("Shrubbery!");
        }

        [Fact]
        public void VerifyPropertyIsSet_WithSpecificValue_WithSetupSet()
        {
            var mock = new Mock<IPropertyManager>();
            var nameUser = new PropertyManagerConsumer(mock.Object);

            mock.SetupSet(m => m.FirstName = "Knights Of Ni!").Verifiable();

            nameUser.ChangeName("Knights Of Ni!");

            mock.Verify();
        }

        [Fact]
        public void TrackPropertyWithSetUpProperty()
        {
            var mock = new Mock<IPropertyManager>();

            //starts tracking firstname so we can set and assert its value later
            mock.SetupProperty(m => m.FirstName);
            mock.Object.FirstName = "Ni!";

            //without setting up property, this cause an error because we didn't start tracking the property on mock
            mock.Object.FirstName.Should().Be("Ni!");

            mock.Object.FirstName = "der wechselnden";
            mock.Object.FirstName.Should().Be("der wechselnden");
        }

        [Fact]
        public void InitializeTrackPropertyWithSetUpProperty()
        {
            var mock = new Mock<IPropertyManager>();
            var nameUser = new PropertyManagerConsumer(mock.Object);

            mock.SetupProperty(m => m.FirstName, "Regina");

            //You can't change the property later with setupGet, but with setupProperty you can.
            //Comment the setup property code and uncomment this to see the difference.
            //mock.SetupGet(m => m.FirstName).Returns("Regina");

            mock.Object.FirstName.Should().Be("Regina");

            mock.Object.FirstName = "Floyd";
            mock.Object.FirstName.Should().Be("Floyd");
        }

        [Fact]
        public void TrackAllPropertiesWithSetupAllProperties()
        {
            var mock = new Mock<IPropertyManager>();

            //here this fails, because we just start tracking first name
            //mock.SetupProperty(m => m.FirstName);

            //this pass because we now tracking all properties
            mock.SetupAllProperties();

            mock.Object.FirstName = "Robert";
            mock.Object.LastName = "Paulson";

            mock.Object.FirstName.Should().Be("Robert");
            mock.Object.LastName.Should().Be("Paulson");
        }
    }
}