using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.UI.Navigation;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.UI.Navigation
{
    [TestFixture]
    public class MenuItemAttributeTester
    {
        private readonly BehaviorChain theChain = new BehaviorChain();

        [Test]
        public void title_as_just_text()
        {
            var key = new MenuItemAttribute("some title").Title;
            key.DefaultValue.ShouldEqual("some title");
            key.Key.ShouldEqual("some title");
            key.ShouldBeOfType<NavigationKey>();
            
        }

        [Test]
        public void title_as_key_and_default_text()
        {
            var key = new MenuItemAttribute("SomeTitle", "some title").Title;
            key.DefaultValue.ShouldEqual("some title");
            key.Key.ShouldEqual("SomeTitle");
            key.ShouldBeOfType<NavigationKey>();
        }

        [Test]
        public void build_registration_for_add_before()
        {
            var registration = new MenuItemAttribute("something"){
                AddBefore = "else"
            }.ToMenuRegistration(theChain).Single().ShouldBeOfType<MenuRegistration>();
            
            
            registration.Strategy.ShouldBeOfType<AddBefore>();
        
        
            registration.Node.Resolve(null);
            registration.Node.BehaviorChain.ShouldBeTheSameAs(theChain);

            registration.Node.Key.ShouldEqual(new NavigationKey("something"));
            registration.Matcher.ShouldEqual(new ByName("else"));
        }

        [Test]
        public void build_registration_for_add_after()
        {
            var registration = new MenuItemAttribute("something")
            {
                AddAfter = "else"
            }.ToMenuRegistration(theChain).Single().ShouldBeOfType<MenuRegistration>();

            registration.Strategy.ShouldBeOfType<AddAfter>();
            registration.Node.Resolve(null);
            registration.Node.BehaviorChain.ShouldBeTheSameAs(theChain);

            registration.Node.Key.ShouldEqual(new NavigationKey("something"));
            registration.Matcher.ShouldEqual(new ByName("else"));
        }

        [Test]
        public void build_registration_for_add_to()
        {
            var registration = new MenuItemAttribute("something")
            {
                AddToMenu = "else"
            }.ToMenuRegistration(theChain).Single().ShouldBeOfType<MenuRegistration>();

            registration.Strategy.ShouldBeOfType<AddChild>();
            registration.Node.Resolve(null);
            registration.Node.BehaviorChain.ShouldBeTheSameAs(theChain);

            registration.Node.Key.ShouldEqual(new NavigationKey("something"));
            registration.Matcher.ShouldEqual(new ByName("else"));
        }

    }
}