using Domain;

namespace Application.Extensions;

public static class ActivityMapper
{
    public static void ToActivityDao(Activity srcActivity, Activity destActivity)
    {
        destActivity.Title = srcActivity.Title;
        destActivity.Date = srcActivity.Date;
        destActivity.Description = srcActivity.Description;
        destActivity.Category = srcActivity.Category;
        destActivity.IsCancelled = srcActivity.IsCancelled;
        destActivity.City = srcActivity.City;
        destActivity.Venue = srcActivity.Venue;
        destActivity.Latitude = srcActivity.Latitude;
        destActivity.Longitude = srcActivity.Longitude;
    }
}
