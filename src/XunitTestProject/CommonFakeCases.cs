using ExampleLibrary;
using ExampleLibrary.Fakes;
using Xunit;

namespace XunitTestProject {
	public class CommonFakeCases {
		[Fact]
		public void XUNIT_StubAnInterfaceOnTheConstructor() {
			var fake = new StubIEmailService();
			string to = "", from = "", subject = "", body = "";
			fake.SendEmailStringStringStringString = (s, s1, arg3, arg4) => {
				to = s;
				from = s1;
				subject = arg3;
				body = arg4;
			};

			var orderService = new OrderService(fake);
			orderService.Ship("100");

			Assert.Equal("customer@localhost.com", to);
			Assert.Equal("shipper@localhost.com", from);
			Assert.Equal("Your order has shipped", subject);
			Assert.Contains("order 100", body);
		}

		[Fact, UseShimContext]
		public void XUNIT_StubAnInterfaceYouDontGetToCreate() {
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
				shim.ShipString = s => { orderServiceWithFake.Ship(s); };
			};

			var orderService = new OrderService();
			orderService.Ship("100");


			Assert.Equal("customer@localhost.com", to);
			Assert.Equal("shipper@localhost.com", from);
			Assert.Equal("Your order has shipped", subject);
			Assert.Contains("order 100", body);
		}

		[Fact, UseShimContext]
		public void XUNIT_StubAConcreteClassYouDontCreate() {
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


			Assert.Equal("customer@localhost.com", to);
			Assert.Equal("shipper@localhost.com", from);
			Assert.Equal("Your order has shipped", subject);
			Assert.Contains("order 100", body);
		}

		[Fact, UseShimContext]
		public void XUNIT_YouCanPassAShimInstanceAsTheInterfaceImplementation() {
			bool wasCalled = false;
			var shim = new ShimCrappyEmailService();
			shim.SendEmailStringStringStringString = (s, s1, arg3, arg4) => { wasCalled = true; };
			var orderService = new OrderService(shim.Instance);
			orderService.Ship("200");
		}

		[Fact, UseShimContext]
		public void XUNIT_YouCanMockAStaticMethod() {
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

			Assert.Equal(1, p1.Id);
			Assert.Equal(2, p2.Id);
		}
	}
}