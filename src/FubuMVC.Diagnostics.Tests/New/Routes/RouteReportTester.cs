using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using FubuMVC.Diagnostics.Features.Chains;
using FubuMVC.Diagnostics.New.Routes;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Diagnostics.Tests.New.Routes
{
    [TestFixture]
    public class RouteReportTester
    {
        private Lazy<RouteReport> _report;
        private BehaviorChain theChain;
        private StubUrlRegistry theUrls;

        [SetUp]
        public void SetUp()
        {
            theChain = new BehaviorChain();
            theUrls = new StubUrlRegistry();
            _report = new Lazy<RouteReport>(() => new RouteReport(theChain, theUrls));
        }

        private RouteReport theReport
        {
            get
            {
                return _report.Value;
            }
        }

        [Test]
        public void resource_type()
        {
            theChain.AddToEnd(ActionCall.For<FakeEndpoint>(x => x.get_something(null)));
            theReport.ResourceType.ShouldEqual(typeof (Output));
        }

        [Test]
        public void input_model()
        {
            theChain.AddToEnd(ActionCall.For<FakeEndpoint>(x => x.get_something(null)));
            theReport.InputModel.ShouldEqual(typeof (Input));
        }

        [Test]
        public void action_with_no_actions()
        {
            theReport.Action.Any().ShouldBeFalse();
        }

        [Test]
        public void one_action()
        {
            theChain.AddToEnd(ActionCall.For<FakeEndpoint>(x => x.get_something(null)));
            theReport.Action.Single().ShouldEqual("FakeEndpoint.get_something(Input input) : Output");
        }

        [Test]
        public void multiple_actions()
        {
            theChain.AddToEnd(ActionCall.For<FakeEndpoint>(x => x.get_something(null)));
            theChain.AddToEnd(ActionCall.For<FakeEndpoint>(x => x.get_another(null)));
            theChain.AddToEnd(ActionCall.For<FakeEndpoint>(x => x.get_third(null)));

            theReport.Action.ShouldHaveTheSameElementsAs(
                "FakeEndpoint.get_something(Input input) : Output", 
                "FakeEndpoint.get_another(Input input) : Output", 
                "FakeEndpoint.get_third(Input input) : Output");
        }

        [Test]
        public void constraints_with_no_route()
        {
            theChain.Route.ShouldBeNull();
            theReport.Constraints.ShouldEqual("N/A");
        }

        [Test]
        public void constraints_with_a_route_but_no_constraints()
        {
            theChain.Route = new RouteDefinition("something");
            theChain.Route.AllowedHttpMethods.Any().ShouldBeFalse();

            theReport.Constraints.ShouldEqual("Any");
        }

        [Test]
        public void one_constraint()
        {
            theChain.Route = new RouteDefinition("something");
            theChain.Route.AddHttpMethodConstraint("GET");

            theReport.Constraints.ShouldEqual("GET");
        }

        [Test]
        public void multiple_constraints()
        {
            theChain.Route = new RouteDefinition("something");
            theChain.Route.AddHttpMethodConstraint("POST");
            theChain.Route.AddHttpMethodConstraint("GET");

            theReport.Constraints.ShouldEqual("GET, POST");
        }

        [Test]
        public void route_when_the_chain_has_no_route()
        {
            // dunno how it gets in this state, but still
            theChain.Route = null;
            theChain.IsPartialOnly = false;

            theReport.Route.ShouldEqual("N/A");
        }

        [Test]
        public void route_when_the_pattern_is_null()
        {
            // dunno how it gets in this state, but still
            theChain.Route = new RouteDefinition(null);
            theChain.IsPartialOnly = false;

            theReport.Route.ShouldEqual("N/A"); 
        }

        [Test]
        public void route_for_a_partial_chain()
        {
            theChain.Route = null;
            theChain.IsPartialOnly = true;

            theReport.Route.ShouldEqual("(partial)");
        }

        [Test]
        public void route_for_the_default_chain()
        {
            theChain.Route = new RouteDefinition(string.Empty);

            theReport.Route.ShouldEqual("(default)");
        }

        [Test]
        public void normal_route()
        {
            theChain.Route = new RouteDefinition("something");

            theReport.Route.ShouldEqual("something");
        }

        [Test]
        public void picks_up_the_chain_url()
        {
            theChain.Route = new RouteDefinition("something");

            theReport.ChainUrl.ShouldEqual(theUrls.UrlFor(new ChainRequest{Id = theChain.UniqueId}));
        }

        [Test]
        public void url_category()
        {
            theChain.UrlCategory.Category = "weird";

            theReport.UrlCategory.ShouldEqual("weird");
        }




        public class Input{}
        public class Output{}

        public class FakeEndpoint
        {
            public Output get_something(Input input)
            {
                return null;
            }

            public Output get_another(Input input)
            {
                return null;
            }

            public Output get_third(Input input)
            {
                return null;
            }
        }
    }
}