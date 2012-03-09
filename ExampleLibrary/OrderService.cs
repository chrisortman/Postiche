using System;
using System.Diagnostics;

namespace ExampleLibrary {
	public class OrderService {

		private readonly IEmailService _emailService;

		public OrderService() {
			_emailService = new CrappyEmailService();
		}

		public OrderService(IEmailService emailService) {
			_emailService = emailService;
		}

		public void Ship(string orderNumber) {
			var to = "customer@localhost.com";
			var from = "shipper@localhost.com";
			var subject = "Your order has shipped";
			var body = @"
Hey!
Guess what, we just shipped your order " + orderNumber;

			_emailService.SendEmail(to, from, subject, body);
		}

		public Product FindProduct(int id) {
			return Product.FindById(id);
		}

		public void ThisMethodIsOnlyCalledFromXunitSoIcanSeeCodeCoverage() {
			Debug.WriteLine("Yay!!!");		
		}
	}

	public class CrappyEmailService : IEmailService {
		public void SendEmail(string to, string @from, string subject, string body) {
			throw new ApplicationException("Man this thing sucks");
		}
	}

	public class Product {
		public int Id { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }

		public static Product FindById(int id) {
			throw new ApplicationException("Where's my database?");
		}
	}
}