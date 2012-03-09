namespace ExampleLibrary {
	public interface IEmailService {
		void SendEmail(string to, string from, string subject, string body);
	}
}