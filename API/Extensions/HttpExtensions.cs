using System.Text.Json;

namespace API.Extensions
{
  public static class HttpExtensions
  {
    public static void AddPaginationHeader(this HttpResponse respones, int currentPage, int itemsPerPage, int totalItems, int totalPages)
    {
      var pagination = new
      {
        currentPage,
        itemsPerPage,
        totalItems,
        totalPages,
      };
      respones.Headers.Add("Pagination", JsonSerializer.Serialize(pagination));
      respones.Headers.Add("Access-Control-Expose-Headers", "Pagination");
    }
  }
}