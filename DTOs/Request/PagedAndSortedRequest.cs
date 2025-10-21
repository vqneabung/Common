using System.ComponentModel.DataAnnotations;

namespace Common.DTOs.Request
{
	public class PagedAndSortedRequest
	{
		[Range(0, int.MaxValue, ErrorMessage = "PageNumber must be greater than 0")]
		public int PageNumber { get; set; } = 1;

        [Range(0, int.MaxValue, ErrorMessage = "PageSize must be greater than 0")]
		public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; }
		public bool SortDescending { get; set; } = false;
	}
}
