using ExampleLibrary;
using ExampleLibrary.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Shims;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MsTestProject {
	[TestClass]
	public class CommonFakeCases {
		
		[TestMethod]
		public void StubAnInterfaceOnTheConstructor() {
			var fake = new StubIEmailService();
			string to="", from="", subject="", body="";
			fake.SendEmailStringStringStringString = (s, s1, arg3, arg4) => {
				to = s;
				from = s1;
				subject = arg3;
				body = arg4;
			};

			var orderService = new OrderService(fake);
			orderService.Ship("100");

			Assert.AreEqual("customer@localhost.com", to);
			Assert.AreEqual("shipper@localhost.com", from);
			Assert.AreEqual("Your order has shipped", subject);
			StringAssert.Contains(body,"order 100");
		}

		[TestMethod]
		public void StubAnInterfaceYouDontGetToCreate() {

			using (ShimsContext.Create()) {
				var fake = new StubIEmailService();
				string to = "", from = "", subject = "", body = "";
				fake.SendEmailStringStringStringString = (s, s1, arg3, arg4) => {
					to = s;
					from = s1;
					subject = arg3;
					body = arg4;
				};
				ShimOrderService.Constructor = @this => {
					var orderServiceWithFake = new OrderService(fake);
					var shim = new ShimOrderService(@this);
					shim.ShipString = s => {
						orderServiceWithFake.Ship(s);
					};
				};

				var orderService = new OrderService();
				orderService.Ship("100");
			

				Assert.AreEqual("customer@localhost.com", to);
				Assert.AreEqual("shipper@localhost.com", from);
				Assert.AreEqual("Your order has shipped", subject);
				StringAssert.Contains(body, "order 100");
			}
		}

		[TestMethod]
		public void StubAConcreteClassYouDontCreate() {

			using (ShimsContext.Create()) {
				var fake = new StubIEmailService();
				string to = "", from = "", subject = "", body = "";
				fake.SendEmailStringStringStringString = (s, s1, arg3, arg4) => {
					to = s;
					from = s1;
					subject = arg3;
					body = arg4;
				};

				ShimCrappyEmailService.AllInstances.SendEmailStringStringStringString = (service, s, arg3, arg4, arg5) => {
					to = s;
					from = arg3;
					subject = arg4;
					body = arg5;
				};
				var orderService = new OrderService();
				orderService.Ship("100");


				Assert.AreEqual("customer@localhost.com", to);
				Assert.AreEqual("shipper@localhost.com", from);
				Assert.AreEqual("Your order has shipped", subject);
				StringAssert.Contains(body, "order 100");
			}
		}

		[TestMethod]
		public void YouCanPassAShimInstanceAsTheInterfaceImplementation() {
			using (ShimsContext.Create()) {
				bool wasCalled = false;
				var shim = new ShimCrappyEmailService();
				shim.SendEmailStringStringStringString = (s, s1, arg3, arg4) => { wasCalled = true; };
				var orderService = new OrderService(shim.Instance);
				orderService.Ship("200");
			}
		}

		[TestMethod]
		public void YouCanMockAStaticMethod() {
			using(ShimsContext.Create()) {
				ShimProduct.FindByIdInt32 = id => {
					return new Product() {
						Id = id,
						Name = "Product " + id,
						Price = 5M
					};
				};

				var orderService = new OrderService();
				var p1 = orderService.FindProduct(1);
				var p2 = orderService.FindProduct(2);

				Assert.AreEqual(1, p1.Id);
				Assert.AreEqual(2, p2.Id);
			}
		}
		
	}
}