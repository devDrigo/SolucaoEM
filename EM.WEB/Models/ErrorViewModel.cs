using EM.DOMAIN;
using EM.REPOSITORY;


namespace EM.WEB.Models
{
	public class ErrorViewModel
	{
		public string? RequestId { get; set; }

		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
	}
}
