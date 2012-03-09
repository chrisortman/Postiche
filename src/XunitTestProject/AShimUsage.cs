using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleLibrary;
using ExampleLibrary.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Xunit;

namespace XunitTestProject
{
    public class AShimUsage
    {
		[Fact]
		public void Xunit_YouCanMockAStaticMethod() {
			using (ShimsContext.Create()) {
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

		[Fact]
		public void ICanHasCodeCoverageForXunit() {
			var orderService = new OrderService(new StubIEmailService());
			orderService.ThisMethodIsOnlyCalledFromXunitSoIcanSeeCodeCoverage();
			Assert.True(true);
		}

		[Fact,UseShimContext]
		public void XunitExtensibilityIsNiceForShimContext() {
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

	[AttributeUsage(AttributeTargets.Method,AllowMultiple = false,Inherited = true)]
	public class UseShimContext : BeforeAfterTestAttribute {
		private IDisposable _context;

		public override void Before(System.Reflection.MethodInfo methodUnderTest) {
			_context = ShimsContext.Create();
		}

		public override void After(System.Reflection.MethodInfo methodUnderTest) {
			_context.Dispose();
		}
	}
}
